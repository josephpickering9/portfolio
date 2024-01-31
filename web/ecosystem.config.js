module.exports = {
  apps: [
    {
      name: 'WorkExperienceWeb',
      port: '3000',
      exec_mode: 'cluster',
      instances: 'max',
      script: './.output/server/index.mjs',
      node_args: '--loader ts-node/esm',
    },
  ],
}
