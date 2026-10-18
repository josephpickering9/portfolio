import { defaultPlugins, defineConfig } from '@hey-api/openapi-ts';

export default defineConfig({
    input: 'http://localhost:5105/swagger/v1/swagger.json',
    output: {
        path: 'api',
        lint: 'eslint',
    },
    plugins: [
        ...defaultPlugins,
        '@hey-api/client-axios',
        {
            baseUrl: 'http://localhost:5105',
            enums: 'typescript',
            name: '@hey-api/typescript',
        },
    ],
});
