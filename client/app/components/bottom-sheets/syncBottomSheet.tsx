import { BottomSheet } from 'heroui-native';
import { CloudAlert, CloudCheck, CloudSync } from 'lucide-react-native';
import React, { useMemo } from 'react';
import { StyleSheet, Text, View } from 'react-native';
import { useAppStore } from '../../stores/appState.store';
import { useBottomSheetsStore } from '../../stores/bottomSheets.store';
import useGlobalStyles from '../../styles/gobalStyles';
import useAppTheme from '../../theme';

const STATUS_COPY: Record<'Syncing' | 'Error' | 'Synced', { label: string; description: string }> = {
    Synced: { label: 'Synced', description: 'Everything is up to date.' },
    Syncing: { label: 'Syncing', description: 'Your changes are being synced now.' },
    Error: { label: 'Sync Error', description: 'We ran into a problem syncing your data.' },
};

const SyncBottomSheet = () => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const { syncStatus } = useAppStore();
    const syncSheetOpen = useBottomSheetsStore((state) => state.syncSheetOpen);
    const setSyncSheetOpen = useBottomSheetsStore((state) => state.setSyncSheetOpen);

    const styles = useMemo(() => StyleSheet.create({
        row: {
            flexDirection: 'row',
            alignItems: 'center',
            gap: 12,
            paddingHorizontal: 20,
            paddingTop: 10,
            paddingBottom: 30,
        },
        description: {
            color: theme.colors.onBackgroundSoft,
            marginTop: 2,
        },
    }), [theme]);

    const { label, description } = STATUS_COPY[syncStatus];

    return (
        <BottomSheet isOpen={syncSheetOpen} onOpenChange={setSyncSheetOpen}>
            <BottomSheet.Portal disableFullWindowOverlay>
                <BottomSheet.Overlay />
                <BottomSheet.Content>
                    <View className="flex-row items-center justify-between px-5 pb-2">
                        <BottomSheet.Title>Sync Status</BottomSheet.Title>
                        <BottomSheet.Close />
                    </View>

                    <View style={styles.row}>
                        {syncStatus === 'Synced' ? (
                            <CloudCheck size={28} color={theme.colors.onBackground} />
                        ) : syncStatus === 'Syncing' ? (
                            <CloudSync size={28} color={theme.colors.onBackground} />
                        ) : (
                            <CloudAlert size={28} color={theme.colors.onBackground} />
                        )}
                        <View style={{ flex: 1 }}>
                            <Text style={{ ...globalStyles.p2, fontWeight: '700' }}>{label}</Text>
                            <Text style={[globalStyles.p4, styles.description]}>{description}</Text>
                        </View>
                    </View>
                </BottomSheet.Content>
            </BottomSheet.Portal>
        </BottomSheet>
    );
};

export default SyncBottomSheet;
