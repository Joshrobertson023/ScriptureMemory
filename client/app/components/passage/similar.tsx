import { Text, View } from "react-native";
import useAppTheme from "../../theme";
import useGlobalStyles from "../../styles/gobalStyles";
import { Passage } from "../../../types/passages/passage";
import { UserPassage } from "../../../types/passages/userPassage";
import { FlatList } from "react-native-gesture-handler";
import PassageContent from "./passageContent";
import Skeleton from "react-native-reanimated-skeleton";

interface SimilarProps {
    reference: string;
    similarPassages?: Passage[];
    isLoading: boolean;
}

const Similar = ({reference, similarPassages = [], isLoading}: SimilarProps) => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const skeletonProps = {
        boneColor: theme.colors.elevation,
        highlightColor: theme.colors.elevation3,
    };
    const skeletonStyle = { width: '100%' as const };
    const skeletonLayout = [
        { width: '100%' as const, height: 88, borderRadius: 10, marginBottom: 10 },
        { width: '100%' as const, height: 88, borderRadius: 10, marginBottom: 10 },
    ];

    return (
        <View style={{marginTop: 20}}>
            <Text style={{...globalStyles.p2, marginBottom: 15}}>
                Similar to {reference}:
            </Text>

            <Skeleton
                isLoading={isLoading}
                containerStyle={skeletonStyle}
                layout={skeletonLayout}
                {...skeletonProps}
            >
                {!isLoading && similarPassages.length === 0 && (
                    <Text style={[globalStyles.p3, { marginTop: 10 }]}>
                        No similar passages available for this passage.
                    </Text>
                )}

                {!isLoading && similarPassages.length > 0 && (
                    <FlatList
                        data={similarPassages}
                        renderItem={({item}) => {
                            const userPassage: UserPassage = {
                                passage: item,
                                id: 0,
                            };

                            return <PassageContent userPassage={userPassage} />;
                        }}
                    />
                )}
            </Skeleton>
        </View>
    )
}

export default Similar;