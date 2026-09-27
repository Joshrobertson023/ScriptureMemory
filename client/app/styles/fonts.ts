import { useFonts } from 'expo-font';
import { LibreBaskerville_400Regular } from '@expo-google-fonts/libre-baskerville/400Regular';
import { Literata_400Regular } from '@expo-google-fonts/literata/400Regular';
import { Lora_400Regular } from '@expo-google-fonts/lora/400Regular';
import { Merriweather_400Regular } from '@expo-google-fonts/merriweather/400Regular';

export function useCustomFonts() {
    const [fontsLoaded] = useFonts({
        'Inter': require('../../assets/fonts/Inter/extras/ttf/Inter-Regular.ttf'),
        'Noto Serif': require('../../assets/fonts/Noto_Serif/static/NotoSerif-Regular.ttf'),
        'Merriweather': Merriweather_400Regular,
        'Lora': Lora_400Regular,
        'Literata': Literata_400Regular,
        'Libre Baskerville': LibreBaskerville_400Regular,
    });
    return fontsLoaded;
}
