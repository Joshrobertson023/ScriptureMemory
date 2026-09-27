export type HighlightColorId = 'yellow' | 'green' | 'blue' | 'pink' | 'orange' | 'purple';

export interface HighlightColorOption {
    id: HighlightColorId;
    label: string;
    hex: string;
}

export const HIGHLIGHT_COLORS: HighlightColorOption[] = [
    { id: 'yellow', label: 'Yellow', hex: '#F5D90A' },
    { id: 'green', label: 'Green', hex: '#8CE071' },
    { id: 'blue', label: 'Blue', hex: '#7FC7FF' },
    { id: 'pink', label: 'Pink', hex: '#F5A3C7' },
    { id: 'orange', label: 'Orange', hex: '#F5A623' },
    { id: 'purple', label: 'Purple', hex: '#C39BF5' },
];

export const DEFAULT_HIGHLIGHT_COLOR: HighlightColorId = 'yellow';

export const getHighlightColor = (id: string): HighlightColorOption =>
    HIGHLIGHT_COLORS.find((c) => c.id === id) ?? HIGHLIGHT_COLORS[0];
