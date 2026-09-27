// // Postinstall script for react-native-reanimated-skeleton to support Expo.
// // Cross-platform replacement for postinstall.sh (works on Windows, macOS, Linux).
//
// const { execSync } = require('child_process');
//
// console.log('Running postinstall script for react-native-reanimated-skeleton to support expo');
//
// function run(command) {
//     console.log(`> ${command}`);
//     execSync(command, { stdio: 'inherit' });
// }
//
// try {
//     // --exclude skips compiled build artifacts (e.g. build/classes/kotlin/...),
//     // which can have filenames long enough to break `git add` on Windows.
//     run('npx patch-package @react-native/gradle-plugin --exclude "(^|/)build/"');
//     run('npx patch-package expo-modules-autolinking --exclude "(^|/)build/"');
// } catch (err) {
//     console.error('postinstall.js failed:', err.message);
//     process.exit(1);
// }