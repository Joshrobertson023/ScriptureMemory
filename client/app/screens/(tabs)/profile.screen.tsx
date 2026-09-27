import { Moon, Sun } from "lucide-react-native";
import { Text, TouchableOpacity, View } from "react-native";
import { SafeAreaView, useSafeAreaInsets } from "react-native-safe-area-context";
import { useUserStore } from "../../stores/user.store";
import useStyles from '../../styles/gobalStyles';
import useAppTheme from "../../theme";
import React, { useState } from "react";
import { Menu, MenuKey } from 'heroui-native';
import { useBibleVersion } from "../../hooks/useBibleVersion";
import { useUserAuthStore } from "../../stores/userAuth.store";
import { getAvailableBibleVersions } from "../../api/bible.api";

export const ProfileScreen = () => {
    const styles = useStyles();
    const theme = useAppTheme();
    const insets = useSafeAreaInsets();
    const setThemePreference = useUserStore((state) => state.setThemePreference);
    const jwt = useUserAuthStore((state) => state.jwt);
    const { version: bibleVersion, setVersion } = useBibleVersion();
    const [bibleVersions, setBibleVersions] = useState<string[]>([]);
    const [loadingVersions, setLoadingVersions] = useState(false);

    // Fetch the list of available translations lazily, the first time the dropdown opens,
    // rather than eagerly on screen load.
    const handleMenuOpenChange = (open: boolean) => {
        if (!open || bibleVersions.length > 0 || loadingVersions) return;

        setLoadingVersions(true);
        getAvailableBibleVersions(jwt)
            .then(setBibleVersions)
            .catch((error) => console.error('Failed to load Bible versions:', error))
            .finally(() => setLoadingVersions(false));
    };

    return (
        <SafeAreaView style={styles.screen}>
            <TouchableOpacity
                style={{ position: 'absolute', top: insets.top + 10, right: 15, padding: 6 }}
                hitSlop={8}
                onPress={() => setThemePreference(theme.dark ? 1 : 2)}
            >
                {theme.dark ? (
                    <Sun size={24} color={theme.colors.onBackground} />
                ) : (
                    <Moon size={24} color={theme.colors.onBackground} />
                )}
            </TouchableOpacity>

            <Text style={styles.p1}>Explore</Text>

            <View style={{marginTop: 20, gap: 8}}>
                <Text style={styles.p3}>Bible version</Text>
                <Menu presentation="bottom-sheet" onOpenChange={handleMenuOpenChange}>
                    <Menu.Trigger>
                        <View style={styles.elevationButton}>
                            <Text style={styles.p3}>{bibleVersion.toUpperCase()}</Text>
                        </View>
                    </Menu.Trigger>
                    <Menu.Portal>
                        <Menu.Overlay />
                        <Menu.Content presentation="bottom-sheet">
                            <Menu.Label>Bible Version</Menu.Label>
                            <Menu.Group
                                selectionMode="single"
                                selectedKeys={new Set<MenuKey>([bibleVersion])}
                                onSelectionChange={(keys) => {
                                    const next = Array.from(keys)[0];
                                    if (typeof next === 'string') setVersion(next);
                                }}
                            >
                                {bibleVersions.map((abbreviation) => (
                                    <Menu.Item key={abbreviation} id={abbreviation}>
                                        <Menu.ItemIndicator />
                                        <Menu.ItemTitle>{abbreviation.toUpperCase()}</Menu.ItemTitle>
                                    </Menu.Item>
                                ))}
                            </Menu.Group>
                        </Menu.Content>
                    </Menu.Portal>
                </Menu>
            </View>
        </SafeAreaView>
    )
}
