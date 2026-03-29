import { useFonts } from 'expo-font';

export function useCustomFonts() {
    const [fontsLoaded] = useFonts({
        'Inter': require('../../assets/fonts/Inter/extras/ttf/Inter-Regular.ttf')
    });
    return fontsLoaded;
}