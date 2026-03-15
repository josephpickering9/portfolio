const fs = require('fs');
const path = require('path');

// eslint-disable-next-line no-console
console.log('📦 Running postbuild script...');

// Paths
const sourceDir = path.join(__dirname, '../node_modules/vue/server-renderer');
const targetDir = path.join(__dirname, '../.output/server/node_modules/vue/server-renderer');

// Check if source exists
if (!fs.existsSync(sourceDir)) {
  // eslint-disable-next-line no-console
  console.error('❌ Source directory not found:', sourceDir);
  process.exit(1);
}

// Create target directory
fs.mkdirSync(targetDir, { recursive: true });

// Copy files
function copyRecursiveSync(src, dest) {
  const exists = fs.existsSync(src);
  const stats = exists && fs.statSync(src);
  const isDirectory = exists && stats.isDirectory();
  
  if (isDirectory) {
    fs.mkdirSync(dest, { recursive: true });
    fs.readdirSync(src).forEach(childItemName => {
      copyRecursiveSync(
        path.join(src, childItemName),
        path.join(dest, childItemName)
      );
    });
  } else {
    fs.copyFileSync(src, dest);
  }
}

copyRecursiveSync(sourceDir, targetDir);

// eslint-disable-next-line no-console
console.log('✅ vue/server-renderer copied successfully to .output/server/node_modules');
