import { Check, X } from 'lucide-react-native';
import React from 'react';
import { Modal, Pressable, Text, View } from 'react-native';
import { HIGHLIGHT_COLORS, HighlightColorId } from '../../styles/highlightColors';
import useGlobalStyles from '../../styles/gobalStyles';
import useAppTheme from '../../theme';

interface HighlightColorDialogProps {
    isOpen: boolean;
    onOpenChange: (open: boolean) => void;
    onSelectColor: (color: HighlightColorId) => void;
    selectedColors?: Set<HighlightColorId | undefined>;
}

const HighlightColorDialog: React.FC<HighlightColorDialogProps> = ({ isOpen, onOpenChange, onSelectColor, selectedColors }) => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();

    return (
        <Modal
            visible={isOpen}
            transparent
            animationType="fade"
            statusBarTranslucent
            onRequestClose={() => onOpenChange(false)}
        >
            <Pressable
                style={{ flex: 1, backgroundColor: '#00000080', justifyContent: 'center', padding: 20 }}
                onPress={() => onOpenChange(false)}
            >
                <Pressable style={{ backgroundColor: theme.colors.elevation, borderRadius: 16, padding: 20 }}>
                    <View style={{ flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between', marginBottom: 16 }}>
                        <Text style={{ ...globalStyles.p2, fontWeight: 700 }}>Highlight Color</Text>
                        <Pressable onPress={() => onOpenChange(false)} hitSlop={10}>
                            <X size={22} color={theme.colors.onBackgroundSoft} strokeWidth={1.5} />
                        </Pressable>
                    </View>

                    <View style={{ flexDirection: 'row', flexWrap: 'wrap', justifyContent: 'space-between', gap: 14 }}>
                        {HIGHLIGHT_COLORS.map((color) => (
                            <Pressable
                                key={color.id}
                                onPress={() => onSelectColor(color.id)}
                                style={{ alignItems: 'center', gap: 6 }}
                            >
                                <View
                                    style={{
                                        width: 44,
                                        height: 44,
                                        borderRadius: 22,
                                        backgroundColor: color.hex,
                                        alignItems: 'center',
                                        justifyContent: 'center',
                                    }}
                                >
                                    {selectedColors?.has(color.id) && (
                                        <Check size={20} color="#00000099" strokeWidth={2.5} />
                                    )}
                                </View>
                                <Text style={{ fontSize: 12, fontFamily: 'Inter', color: theme.colors.onBackgroundSoft }}>
                                    {color.label}
                                </Text>
                            </Pressable>
                        ))}
                    </View>
                </Pressable>
            </Pressable>
        </Modal>
    );
};

export default HighlightColorDialog;
