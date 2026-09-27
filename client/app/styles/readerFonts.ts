export type ReaderFontId = 'inter' | 'notoSerif' | 'merriweather' | 'lora' | 'literata' | 'libreBaskerville';

export interface ReaderFontOption {
    id: ReaderFontId;
    label: string;
    fontFamily: string;
}

export const READER_FONTS: ReaderFontOption[] = [
    { id: 'inter', label: 'Inter', fontFamily: 'Inter' },
    { id: 'notoSerif', label: 'Noto Serif', fontFamily: 'Noto Serif' },
    { id: 'merriweather', label: 'Merriweather', fontFamily: 'Merriweather' },
    { id: 'lora', label: 'Lora', fontFamily: 'Lora' },
    { id: 'literata', label: 'Literata', fontFamily: 'Literata' },
    { id: 'libreBaskerville', label: 'Libre Baskerville', fontFamily: 'Libre Baskerville' },
];

export type ReaderBackgroundId = 'default' | 'trueBlack' | 'white' | 'cream';

export interface ReaderBackgroundOption {
    id: ReaderBackgroundId;
    label: string;
    backgroundColor: string;
    textColor: string;
    hintColor: string;
    swatchBorderColor: string;
}

export const READER_BACKGROUNDS: ReaderBackgroundOption[] = [
    {
        id: 'default',
        label: 'Dark',
        backgroundColor: '#101010',
        textColor: '#F4F4F4',
        hintColor: '#959595',
        swatchBorderColor: '#3A3A3A',
    },
    {
        id: 'trueBlack',
        label: 'True Black',
        backgroundColor: '#000000',
        textColor: '#F4F4F4',
        hintColor: '#8A8A8A',
        swatchBorderColor: '#3A3A3A',
    },
    {
        id: 'white',
        label: 'White',
        backgroundColor: '#FFFFFF',
        textColor: '#1A1A1A',
        hintColor: '#6B6B6B',
        swatchBorderColor: '#D9D9D9',
    },
    {
        id: 'cream',
        label: 'Cream',
        backgroundColor: '#F6ECD8',
        textColor: '#3B2F22',
        hintColor: '#8A7A63',
        swatchBorderColor: '#E0D2B4',
    },
];

export const MIN_READER_FONT_SIZE = 15;
export const MAX_READER_FONT_SIZE = 26;

export const MIN_READER_MARGIN = 10;
export const MAX_READER_MARGIN = 45;

export const getReaderFont = (id: ReaderFontId): ReaderFontOption =>
    READER_FONTS.find((f) => f.id === id) ?? READER_FONTS[1];

export const getReaderBackground = (id: ReaderBackgroundId): ReaderBackgroundOption =>
    READER_BACKGROUNDS.find((b) => b.id === id) ?? READER_BACKGROUNDS[0];
