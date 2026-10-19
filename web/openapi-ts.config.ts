import { resolve } from 'node:path';
import { defaultPlugins, defineConfig } from '@hey-api/openapi-ts';

export default defineConfig({
    input: resolve('.openapi.json'),
    output: {
        path: 'api',
    },
    plugins: [
        ...defaultPlugins,
        {
            baseUrl: 'http://localhost:5105',
            name: '@hey-api/client-axios',
        },
        {
            enums: 'typescript',
            name: '@hey-api/typescript',
        },
    ],
});
