#!/bin/bash

# Postbuild script to fix Nuxt 4 bundling issue
# Copies missing vue/server-renderer into .output/server/node_modules

echo "📦 Copying vue/server-renderer to .output/server/node_modules..."

# Create the server-renderer directory if it doesn't exist
mkdir -p .output/server/node_modules/vue/server-renderer

# Copy the server-renderer files from project node_modules
cp -r node_modules/vue/server-renderer/* .output/server/node_modules/vue/server-renderer/

echo "✅ vue/server-renderer copied successfully"
