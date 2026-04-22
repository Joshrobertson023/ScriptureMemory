import { useColorScheme, Appearance } from 'react-native';
import { DefaultTheme } from '@react-navigation/native';
import { useUserStore } from './stores/user.store';

export default function useAppTheme() {
    const systemScheme = useColorScheme();
    const themePreference = useUserStore((state) => state.user.preferences.theme);
    const scheme = themePreference === 0 ? (systemScheme || 'light') : themePreference;

    return scheme === 'dark' ? {
      ...DefaultTheme,
      colors: {
        ...DefaultTheme.colors,
        primary: '#834343',
        brightPrimary: '#CF4F4F',
        background: '#101010',
        background2: '#222222',
        onBackground: '#F4F4F4',
        onBackgroundSoft: '#D9D9D9',
        onBackgroundSuperSoft: '#C3C3C3',
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
        brightPrimary: '#CF4F4F',
        background: '#101010',
        background2: '#1e1e1e',
        onBackground: '#F4F4F4',
        onBackgroundSoft: '#D9D9D9',
        onBackgroundSuperSoft: '#C3C3C3',
        elevation: '#1f1f1f',
        elevation2: '#383838',
        elevation3: '#696969',
        white: '#F4F4F4',
        verseHint: '#959595ff',
        inactiveTab: '#959595ff'
      }
    };
}