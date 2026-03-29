import { useColorScheme, Appearance } from 'react-native';
import { DefaultTheme } from '@react-navigation/native';
import { useAppStore } from './store';

export default function useAppTheme() {
    const systemScheme = useColorScheme();
    const themePreference = useAppStore((state) => state.themePreference);
    const scheme = themePreference === 'system' ? (systemScheme || 'light') : themePreference;

    return scheme === 'dark' ? {
      ...DefaultTheme,
      colors: {
        ...DefaultTheme.colors,
        primary: '#834343',
        background: '#181818',
        background2: '#222222',
        onBackground: '#F4F4F4',
        elevation: '#2E2E2E',
        elevation2: '#494949',
        elevation3: '#696969',
        white: '#F4F4F4',
        verseHint: '#959595ff',
        inactiveTab: 'rgb(207, 207, 207)'
      }
    } : {
      ...DefaultTheme,
      colors: {
        ...DefaultTheme.colors,
        primary: '#834343',
        background: '#181818',
        background2: '#222222',
        onBackground: '#F4F4F4',
        elevation: '#2E2E2E',
        elevation2: '#494949',
        elevation3: '#696969',
        white: '#F4F4F4',
        verseHint: '#959595ff',
        inactiveTab: '#959595ff'
      }
    };
}