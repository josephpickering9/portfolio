// @ts-check
import withNuxt from './.nuxt/eslint.config.mjs'
import unusedImports from 'eslint-plugin-unused-imports'

export default withNuxt()
  // Add ignores
  .append({
    ignores: [
      '.github/*',
      '.husky/*',
      '.nuxt/*',
      '.output/*',
      'dist/*',
      'node_modules/*',
      'static/*'
    ]
  })
  // Add plugins and rules
  .append({
    plugins: {
      'unused-imports': unusedImports
    },
    rules: {
      // TypeScript specific rules
      '@typescript-eslint/no-var-requires': 'off',
      '@typescript-eslint/ban-ts-comment': 'off',
      '@typescript-eslint/no-explicit-any': 'off',
      '@typescript-eslint/no-unused-vars': 'off',

      // Vue specific rules
      'vue/no-v-html': 'off',
      'vue/multi-word-component-names': 'off',
      'vue/component-name-in-template-casing': [
        'error',
        'PascalCase',
        {
          registeredComponentsOnly: false,
          ignores: [],
        }
      ],
      'vue/order-in-components': [
        'error',
        {
          order: [
            'el',
            'name',
            'key',
            'parent',
            'functional',
            ['delimiters', 'comments'],
            ['components', 'directives', 'filters'],
            'extends',
            'mixins',
            ['provide', 'inject'],
            'ROUTER_GUARDS',
            'layout',
            'middleware',
            'validate',
            'scrollToTop',
            'transition',
            'loading',
            'inheritAttrs',
            'model',
            ['props', 'propsData'],
            'emits',
            'setup',
            'asyncData',
            'data',
            'fetch',
            'head',
            'computed',
            'validations',
            'LIFECYCLE_HOOKS',
            'methods',
            'watch',
            'watchQuery',
            ['template', 'render'],
            'renderError',
          ],
        },
      ],
      'vue/no-unused-vars': ['error'],
      'vue/match-component-file-name': [
        'error',
        {
          extensions: ['vue'],
          shouldMatchCase: false,
        },
      ],
      'vue/padding-line-between-blocks': ['error', 'always'],

      // General rules
      'no-this-alias': 'off',
      'no-console': ['error'],
      'no-alert': ['error'],
      'no-restricted-imports': [
        'error',
        {
          patterns: [
            {
              group: ['~'],
              message: 'Please use an explicit path in Storybook imports. 😡',
            },
          ],
        },
      ],

      // Unused imports rules
      'unused-imports/no-unused-imports': 'error',
      'unused-imports/no-unused-vars': [
        'warn',
        { vars: 'all', varsIgnorePattern: '^', args: 'after-used', argsIgnorePattern: '^' },
      ],
    }
  })
