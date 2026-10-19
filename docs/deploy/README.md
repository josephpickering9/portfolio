# Deployment

Both sides deploy to one DigitalOcean droplet from GitHub Actions on push to `main`, and only after
their tests and the shared E2E suite pass:

```
push to main ─┬─ API workflow ─ build & test ─┬─ E2E ─ API Deploy ─ api/deploy/activate.sh
              └─ Web workflow ─ lint & test  ─┴─ E2E ─ Web Deploy ─ web/deploy/activate.sh
```

Each deploy uploads a tarball to `/var/www/experience/{api,web}/releases/<sha>`, runs that release's
`deploy/activate.sh` over SSH, then keeps the two most recent releases.

## API (`api/deploy/activate.sh`)

1. Writes `appsettings.json` (connection string, Auth0, CORS origin, Vertex AI) and the Vertex service
   account into the release, both `chmod 600`.
2. Applies migrations with the self-contained `efbundle` built in CI.
3. Installs or updates the `portfolio-api` systemd unit, which runs `current/Portfolio.Api.dll` on
   `http://localhost:5000` and restarts it if it crashes or the droplet reboots. The first deploy
   also stops the old `nohup` instance.
4. Points `current` at the new release, restarts the service and polls `/health`.
5. If `/health` never passes, it points `current` back at the previous release, restarts that and
   fails the job. **Migrations are not rolled back**, so a migration must stay compatible with the
   previous release (add columns, don't rename or drop them in the same release that stops using them).

Logs: `/var/www/experience/api/logs/api.log`, or `journalctl -u portfolio-api`.

## Web (`web/deploy/activate.sh`)

Writes `.env` from the `NUXT_ENV` secret, points `current` at the new release, starts it under PM2 as
`PortfolioWeb` (deleting the old `WorkExperienceWeb` process if it's still there), polls
`http://localhost:3000/` and rolls back to the previous release the same way if it never responds.

## nginx

nginx isn't managed from this repo, so these changes have to be made on the droplet by hand. They
matter: as of October 2026 nginx serves `/_nuxt/` itself with `expires 7d` and no compression for
JavaScript, which is most of the gap between the local and live Lighthouse scores.

Once, in the `http {}` block of `/etc/nginx/nginx.conf`. Ubuntu's default file already has
`gzip on;` and these lines commented out, so uncomment and edit them there. Adding them in a
`conf.d` file as well fails `nginx -t` with a duplicate-directive error.

```nginx
server_tokens off;

gzip on;
gzip_vary on;
gzip_proxied any;
gzip_comp_level 6;
gzip_min_length 1024;
gzip_types text/plain text/css text/xml application/javascript application/json application/xml
           application/manifest+json image/svg+xml;
```

Frontend server block (`josephpickering.co.uk`). Keep the certbot-managed `ssl_*` lines as they are:

```nginx
listen 443 ssl http2;

# Hashed build assets: their names change whenever their content does, so cache them for a year.
# The build writes .gz (and .br) copies next to each file; gzip_static serves the .gz one.
location /_nuxt/ {
    root /var/www/experience/web/current/.output/public;
    gzip_static on;
    add_header Cache-Control "public, max-age=31536000, immutable";
    access_log off;
    try_files $uri =404;
}

location / {
    proxy_pass http://127.0.0.1:3000;
    proxy_http_version 1.1;
    proxy_set_header Host $host;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
}
```

Replace the existing `/_nuxt/` location (the one that sets `expires 7d`) rather than adding a second
one. The security headers (CSP with nonces, HSTS, `X-Content-Type-Options`, …) come from
`nuxt-security`, so nginx shouldn't add its own copies.

API server block (`api.experience.josephpickering.co.uk`):

```nginx
listen 443 ssl http2;

location / {
    proxy_pass http://127.0.0.1:5000;
    proxy_set_header Host $host;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
}
```

The two `X-Forwarded-*` headers are required, not optional: the API trusts them from loopback
(`Extensions/SecurityExtensions.cs`) to rate-limit AI search per visitor and to send HSTS. Without
`X-Forwarded-For`, every visitor shares one rate-limit bucket.

Check and reload with `nginx -t && systemctl reload nginx`, then confirm:

```bash
curl -sI -H 'Accept-Encoding: gzip' https://josephpickering.co.uk/_nuxt/<any-hashed-file>.js | grep -iE 'content-encoding|cache-control'
curl -sI https://josephpickering.co.uk/ | grep -iE 'server|content-security-policy'
```

## One-off checks after the first deploy of this setup

- `systemctl status portfolio-api` shows the API running under systemd, and nothing else is listening
  on port 5000 (`lsof -i :5000`).
- `pm2 ls` shows `PortfolioWeb` only.
- Before this change, the clean-up step's `ARTIFACTS_PATH` was never passed to the SSH session, so its
  `ls -t | tail -n +3 | xargs rm -rf` ran in root's home directory on every deploy. Check `/root` for
  anything that should be there.
