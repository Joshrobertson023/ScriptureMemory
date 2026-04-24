import { Text, View } from "react-native";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";
import { Passage } from "../../../types/passages/passage";
import Skeleton from "react-native-reanimated-skeleton";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";
import { useBottomSheetStack } from "../../hooks/useBottomSheetStack";
import { UserPassage } from "../../../types/passages/userPassage";
import { CrossReferenceGroup } from "../../../types/verse/verseCard";

export interface CrossReferencesProps {
    crossReferences: CrossReferenceGroup[];
    loading: boolean;
}

const CrossReferences = ({ crossReferences, loading }: CrossReferencesProps) => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const skeletonProps = {
        boneColor: theme.colors.elevation,
        highlightColor: theme.colors.elevation3,
    };
    const {
        setPassageSheetOpen,
    } = useBottomSheetsStore();

    const skeletonStyle = { width: '90%' as const };
    const skeletonLayout = [{ width: '100%' as const, height: 75, borderRadius: 4 }];

    const {
        goToNextPassage
    } = useBottomSheetStack();

    const handleCrossReferencePress = (p: Passage) => {
        const up: UserPassage = {
            passage: p,
        }
        goToNextPassage(up);
    };

    return (
        <Skeleton
            isLoading={loading}
            containerStyle={skeletonStyle}
            layout={skeletonLayout}
            {...skeletonProps}
        >
            <View>
                <Text style={globalStyles.p2}>
                    Cross References
                </Text>
                <View style={{ height: 10 }} />
                {!loading && crossReferences.length === 0 && (
                    <Text style={globalStyles.p3}>
                        No cross references available for this passage.
                    </Text>
                )}

                {!loading && crossReferences.length > 0 && (
                    <View style={{ gap: 8 }}>
                        {crossReferences.map((group) => (
                            <Text
                                key={`${group.fromVerse.id}-${group.fromVerse.reference.readableReference}`}
                                style={{ ...globalStyles.p3, lineHeight: 25 }}
                            >
                                {crossReferences.length > 1 && (
                                    <Text style={{ ...globalStyles.p3, lineHeight: 25 }}>
                                        {'v. ' + group.fromVerse.reference.verses.at(0) + ': '}
                                    </Text>
                                )} 
                                {group.crossReferences.map((passage, index) => (
                                    <Text
                                        key={`${passage.reference.readableReference}-${index}`}
                                        numberOfLines={1}
                                        onPress={() => handleCrossReferencePress(passage)}
                                    >
                                        <Text
                                            style={{
                                                ...globalStyles.p3,
                                                textDecorationLine: 'underline',
                                                lineHeight: 25,
                                            }}
                                        >
                                            {passage.reference.readableReference}
                                        </Text>
                                        {index < group.crossReferences.length - 1 ? '; ' : ''}
                                    </Text>
                                ))}
                            </Text>
                        ))}
                    </View>
                )}
            </View>
        </Skeleton>
    );
};

export default CrossReferences;