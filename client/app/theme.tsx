import { DarkTheme, DefaultTheme } from '@react-navigation/native';
import { useUserStore } from './stores/user.store';

export const darkTheme = {
    ...DarkTheme,
    colors: {
        ...DarkTheme.colors,
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
};

export const lightTheme = {
    ...DefaultTheme,
    colors: {
        ...DefaultTheme.colors,
        primary: '#834343',
        brightPrimary: '#CF4F4F',
        background: '#FAFAFA',
        background2: '#EFEFEF',
        onBackground: '#141414',
        onBackgroundSoft: '#2B2B2B',
        onBackgroundSuperSoft: '#474747',
        elevation: '#E8E8E8',
        elevation2: '#D4D4D4',
        elevation3: '#ABABAB',
        white: '#F4F4F4',
        verseHint: '#8C8C8C',
        inactiveTab: '#8C8C8C'
    }
};

export function useIsDarkMode() {
    return useUserStore((state) => state.resolvedDark);
}

export default function useAppTheme() {
    return useIsDarkMode() ? darkTheme : lightTheme;
}
