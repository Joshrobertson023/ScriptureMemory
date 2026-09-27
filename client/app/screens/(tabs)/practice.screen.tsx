import { useLiveQuery } from "drizzle-orm/expo-sqlite";
import { Brain } from "lucide-react-native";
import React, { useCallback, useMemo } from "react";
import { SectionList, Text, TouchableOpacity, View } from "react-native";
import { UserPassage } from "../../../types/passages/userPassage";
import PassageContent from "../../components/passage/passageContent";
import { mapPassageRow } from "../../database/repositories/collections.repository";
import { practiceListQuery } from "../../database/repositories/practice.repository";
import { pushPracticeSessionRoute } from "../../navigation";
import useGlobalStyles from '../../styles/gobalStyles';
import useAppTheme from "../../theme";

export const PracticeScreen = () => {
    const styles = useGlobalStyles();
    const theme = useAppTheme();
    const { data } = useLiveQuery(practiceListQuery());

    const sections = useMemo(() => {
        const overdue: UserPassage[] = [];
        const upcoming: UserPassage[] = [];
        const seen = new Set<string>();
        const now = Date.now();

        for (const { practice, passage } of data ?? []) {
            if (seen.has(practice.passageId)) continue;
            seen.add(practice.passageId);

            const item = mapPassageRow(passage);
            if (item.type !== 'passage') continue;

            if (practice.nextDueDate && new Date(practice.nextDueDate).getTime() <= now) {
                overdue.push(item.passage);
            } else {
                upcoming.push(item.passage);
            }
        }

        return [
            { title: 'Overdue', data: overdue },
            { title: 'Upcoming', data: upcoming },
        ].filter((section) => section.data.length > 0);
    }, [data]);

    const renderItem = useCallback(({ item }: { item: UserPassage }) => (
        <TouchableOpacity
            style={{ backgroundColor: theme.colors.elevation, borderRadius: 10, padding: 14, marginBottom: 10 }}
            onPress={() => pushPracticeSessionRoute(item)}
        >
            <PassageContent userPassage={item} />
        </TouchableOpacity>
    ), [theme]);

    return (
        <SectionList
            style={{ flex: 1, backgroundColor: theme.colors.background }}
            contentContainerStyle={{ padding: 15, flexGrow: 1 }}
            sections={sections}
            keyExtractor={(item) => item.id ?? item.passage.reference.readableReference}
            renderItem={renderItem}
            renderSectionHeader={({ section }) => (
                <Text style={{ ...styles.p2, fontWeight: 700, marginTop: 10, marginBottom: 10 }}>{section.title}</Text>
            )}
            stickySectionHeadersEnabled={false}
            ListEmptyComponent={
                <View style={{ flex: 1, justifyContent: 'center', alignItems: 'center', gap: 6, paddingHorizontal: 30 }}>
                    <Brain size={48} color={theme.colors.onBackgroundSoft} strokeWidth={1.3} />
                    <Text style={{ ...styles.p3, textAlign: 'center' }}>No passages being memorized yet. Tap "Practice" on a passage to start.</Text>
                </View>
            }
            ListFooterComponent={<View style={{ height: 100 }} />}
        />
    );
};
