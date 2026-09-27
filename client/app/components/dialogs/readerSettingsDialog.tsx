import { Button, Dialog, Label, Radio, RadioGroup, Separator, Slider, Surface } from 'heroui-native';
import React, { Fragment } from 'react';
import { Pressable, ScrollView, Text, View, useWindowDimensions } from 'react-native';
import { useReaderSettingsStore } from '../../stores/readerSettings.store';
import {
    MAX_READER_FONT_SIZE,
    MIN_READER_FONT_SIZE,
    READER_BACKGROUNDS,
    READER_FONTS,
} from '../../styles/readerFonts';
import useAppTheme from '../../theme';

interface ReaderSettingsDialogProps {
    isOpen: boolean;
    onOpenChange: (open: boolean) => void;
}

const ReaderSettingsDialog: React.FC<ReaderSettingsDialogProps> = ({ isOpen, onOpenChange }) => {
    const theme = useAppTheme();
    const { height: windowHeight } = useWindowDimensions();
    const {
        fontId,
        fontSize,
        background,
        setFontId,
        setFontSize,
        setBackground,
        reset,
    } = useReaderSettingsStore();

    return (
        <Dialog isOpen={isOpen} onOpenChange={onOpenChange}>
            <Dialog.Portal>
                <Dialog.Overlay />
                <Dialog.Content className="w-full" isSwipeable={false}>
                    <View className="flex-row items-center justify-between mb-4">
                        <Dialog.Title>Reading Preferences</Dialog.Title>
                        <Dialog.Close variant="ghost" />
                    </View>

                    <ScrollView
                        style={{ maxHeight: windowHeight * 0.6 }}
                        contentContainerStyle={{ gap: 22, paddingBottom: 4 }}
                        showsVerticalScrollIndicator={false}
                        nestedScrollEnabled
                    >
                        {/* Font size */}
                        <Slider
                            value={fontSize}
                            minValue={MIN_READER_FONT_SIZE}
                            maxValue={MAX_READER_FONT_SIZE}
                            step={1}
                            onChange={(v) => setFontSize(v as number)}
                        >
                            <View className="flex-row items-center justify-between">
                                <Label>Font Size</Label>
                                <Slider.Output />
                            </View>
                            <Slider.Track>
                                <Slider.Fill />
                                <Slider.Thumb />
                            </Slider.Track>
                        </Slider>

                        <Separator />

                        {/* Font family */}
                        <View style={{ gap: 10, paddingTop: 10, paddingBottom: 10 }}>
                            <Label>Font</Label>
                            <Surface variant="secondary" className="p-0">
                                <RadioGroup value={fontId} onValueChange={(v) => setFontId(v as typeof fontId)}>
                                    {READER_FONTS.map((font, index) => (
                                        <Fragment key={font.id}>
                                            <RadioGroup.Item value={font.id} className="px-4 py-3">
                                                <View className="flex-row items-center gap-3 flex-1">
                                                    <Text
                                                        style={{
                                                            fontFamily: font.fontFamily,
                                                            fontSize: 20,
                                                            width: 32,
                                                            color: theme.colors.onBackground,
                                                        }}
                                                    >
                                                        Aa
                                                    </Text>
                                                    <Label>{font.label}</Label>
                                                </View>
                                                <Radio />
                                            </RadioGroup.Item>
                                            {index < READER_FONTS.length - 1 && <Separator />}
                                        </Fragment>
                                    ))}
                                </RadioGroup>
                            </Surface>
                        </View>

                        <Separator />

                        {/* Background */}
                        <View style={{ gap: 10 }}>
                            <Label>Background</Label>
                            <View className="flex-row justify-between">
                                {READER_BACKGROUNDS.map((bg) => {
                                    const selected = bg.id === background;
                                    return (
                                        <Pressable
                                            key={bg.id}
                                            onPress={() => setBackground(bg.id)}
                                            style={{ alignItems: 'center', gap: 6 }}
                                        >
                                            <View
                                                style={{
                                                    width: 52,
                                                    height: 52,
                                                    borderRadius: 26,
                                                    backgroundColor: bg.backgroundColor,
                                                    borderWidth: selected ? 2.5 : 1,
                                                    borderColor: selected ? theme.colors.brightPrimary : bg.swatchBorderColor,
                                                    alignItems: 'center',
                                                    justifyContent: 'center',
                                                }}
                                            >
                                                <Text style={{ color: bg.textColor, fontFamily: 'Inter', fontWeight: '700' }}>
                                                    Aa
                                                </Text>
                                            </View>
                                            <Text style={{ fontSize: 12, fontFamily: 'Inter', color: theme.colors.onBackgroundSoft }}>
                                                {bg.label}
                                            </Text>
                                        </Pressable>
                                    );
                                })}
                            </View>
                        </View>

                        <Separator />

                        <Button variant="ghost" size="sm" className="self-center" onPress={reset}>
                            <Button.Label>Reset to Default</Button.Label>
                        </Button>
                    </ScrollView>
                </Dialog.Content>
            </Dialog.Portal>
        </Dialog>
    );
};

export default ReaderSettingsDialog;
