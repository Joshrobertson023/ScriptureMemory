import { Spinner } from "heroui-native";
import React from "react";
import { Text, View } from "react-native";
import Skeleton from "react-native-reanimated-skeleton";
import { Passage } from "../../../types/passages/passage";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";
import AddPassageContent from "./addPassageContent";

interface SimilarProps {
    reference: string;
    similarPassages?: Passage[];
    isLoading: boolean;
    isFetchingMore: boolean;
}

const Similar = React.memo(({reference, similarPassages = [], isLoading, isFetchingMore}: SimilarProps) => {
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

                {!isLoading && similarPassages.map((item, index) => (
                    <AddPassageContent key={`${item.reference.readableReference}-${index}`} passage={item} />
                ))}
            </Skeleton>

            {isFetchingMore && <Spinner style={{ marginTop: 10, alignSelf: 'center' }} />}        </View>
    )
})

export default Similar;
