import { Dialog, Label, Radio, RadioGroup, Separator, Surface } from 'heroui-native';
import React, { Fragment, useEffect, useState } from 'react';
import { ScrollView, View, useWindowDimensions } from 'react-native';
import { getAvailableBibleVersions } from '../../api/bible.api';
import { useBibleVersion } from '../../hooks/useBibleVersion';
import { useReaderSettingsStore } from '../../stores/readerSettings.store';
import { useUserAuthStore } from '../../stores/userAuth.store';

interface ReadTranslationDialogProps {
    isOpen: boolean;
    onOpenChange: (open: boolean) => void;
}

const ReadTranslationDialog: React.FC<ReadTranslationDialogProps> = ({ isOpen, onOpenChange }) => {
    const { height: windowHeight } = useWindowDimensions();
    const jwt = useUserAuthStore((state) => state.jwt);
    const { version: preferredVersion } = useBibleVersion();
    const { translationOverride, setTranslationOverride } = useReaderSettingsStore();

    const [versions, setVersions] = useState<string[]>([]);
    const [loadingVersions, setLoadingVersions] = useState(false);

    useEffect(() => {
        if (!isOpen || versions.length > 0 || loadingVersions) return;

        setLoadingVersions(true);
        getAvailableBibleVersions(jwt)
            .then(setVersions)
            .catch((error) => console.error('Failed to load Bible versions:', error))
            .finally(() => setLoadingVersions(false));
    }, [isOpen]);

    const selectedVersion = translationOverride ?? preferredVersion;

    return (
        <Dialog isOpen={isOpen} onOpenChange={onOpenChange}>
            <Dialog.Portal>
                <Dialog.Overlay />
                <Dialog.Content className="w-full" isSwipeable={false}>
                    <View className="flex-row items-center justify-between mb-4">
                        <Dialog.Title>Reading Translation</Dialog.Title>
                        <Dialog.Close variant="ghost" />
                    </View>

                    <ScrollView
                        style={{ maxHeight: windowHeight * 0.6 }}
                        contentContainerStyle={{ gap: 22, paddingBottom: 4 }}
                        showsVerticalScrollIndicator={false}
                        nestedScrollEnabled
                    >
                        <View style={{ gap: 10 }}>
                            <Surface variant="secondary" className="p-0">
                                <RadioGroup
                                    value={selectedVersion}
                                    onValueChange={(v) => setTranslationOverride(v as string)}
                                >
                                    {versions.map((abbreviation, index) => (
                                        <Fragment key={abbreviation}>
                                            <RadioGroup.Item value={abbreviation} className="px-4 py-3">
                                                <Label>{abbreviation.toUpperCase()}</Label>
                                                <Radio />
                                            </RadioGroup.Item>
                                            {index < versions.length - 1 && <Separator />}
                                        </Fragment>
                                    ))}
                                </RadioGroup>
                            </Surface>
                        </View>
                    </ScrollView>
                </Dialog.Content>
            </Dialog.Portal>
        </Dialog>
    );
};

export default ReadTranslationDialog;
