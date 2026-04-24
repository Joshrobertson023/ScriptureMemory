import { StyleSheet, View, Text } from "react-native";
import { UserPassage } from "../../../types/passages/userPassage";
import useAppTheme from "../../theme";
import useGlobalStyles from "../../styles/gobalStyles";
import { useMemo } from "react";
import Skeleton from "react-native-reanimated-skeleton";
import { VerseCardResponse } from "../../../types/verse/verseCard";
import { Brain, Check, Clock, Users } from "lucide-react-native";
import { useCollectionsStore } from "../../stores/collections.store";

interface PassageSheetMetadataProps {
    passage: UserPassage;
    data: VerseCardResponse;
    loading: boolean;
}

const PassageSheetMetadata = ({ passage, data, loading }: PassageSheetMetadataProps) => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const { userCollections } = useCollectionsStore();

    const passageVerseIds = useMemo(
        () => new Set(passage.passage.verses.map((verse) => verse.id)),
        [passage]
    );

    const collectionsCount = userCollections.filter(c =>
        c.items.some(i =>
            i.type === 'passage' && i.passage.passage.verses.some((verse) => passageVerseIds.has(verse.id))
        )
    ).length;

    const styles = useMemo(() => StyleSheet.create({
        container: { flexDirection: 'row' },
        column: { flex: 1, gap: 12 },
        item: { flexDirection: 'row', alignItems: 'center', gap: 8 },
        text: { flex: 1 }
    }), [theme]);

    const skeletonProps = {
        boneColor: theme.colors.elevation,
        highlightColor: theme.colors.elevation3,
    };

    const skeletonStyle = { height: 16, width: '90%' as const };
    const skeletonLayout = [{ width: '100%' as const, height: 16, borderRadius: 4 }];

    return (
        <View style={styles.container}>
            <View style={styles.column}>

                <View style={styles.item}>
                    <Check size={15} color={theme.colors.onBackground} />
                    <Text style={[globalStyles.p4, styles.text]}>
                        In {collectionsCount} {collectionsCount === 1 ? 'collection' : 'collections'}
                    </Text>
                </View>

                {(passage.timesMemorized ?? 0) > 0 && (
                    <View style={styles.item}>
                        <Brain size={15} color={theme.colors.onBackground} />
                        <Text style={[globalStyles.p4, styles.text]}>
                            Practiced {passage.timesMemorized}x
                        </Text>
                    </View>
                )}

                {passage.dueDate && (
                    <View style={styles.item}>
                        <Clock size={15} color={theme.colors.onBackground} />
                        <Text style={[globalStyles.p4, styles.text]}>
                            Next due {new Date(passage.dueDate).toDateString()}
                        </Text>
                    </View>
                )}

            </View>
            <View style={styles.column}>

                <Skeleton isLoading={loading} containerStyle={skeletonStyle}
                    layout={skeletonLayout} {...skeletonProps}>
                    <View style={styles.item}>
                        <Users size={15} color={theme.colors.onBackground} />
                        <Text style={[globalStyles.p4, styles.text]}>
                            {data.totalSaved}{' '}
                            {data.totalSaved === 1 ? 'Person' : 'People'} saved this verse
                        </Text>
                    </View>
                </Skeleton>

                <Skeleton isLoading={loading} containerStyle={skeletonStyle}
                    layout={skeletonLayout} {...skeletonProps}>
                    <View style={styles.item}>
                        <Brain size={15} color={theme.colors.onBackground} />
                        <Text style={[globalStyles.p4, styles.text]}>
                            {data.totalMemorized}{' '}
                            {data.totalMemorized === 1 ? 'Person' : 'People'} practiced this verse
                        </Text>
                    </View>
                </Skeleton>

            </View>
        </View>
    );
};

export default PassageSheetMetadata;