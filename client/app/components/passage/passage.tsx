import { StyleSheet, View } from "react-native";
import { Passage } from "../../../types/passages/passage";
import { UserPassage } from "../../../types/passages/userPassage";
import { useIsActive, useReorderableDrag } from "react-native-reorderable-list";
import Swipeable from 'react-native-gesture-handler/ReanimatedSwipeable';
import PassageContent from "./passageContent";
import { TouchableOpacity } from "react-native";
import { Trash } from "lucide-react-native";
import useAppTheme from "../../theme";
import { useCollectionsStore } from "../../stores/collections.store";

interface PassageProps {
    userPassage: UserPassage;
    itemId: number;
    collectionId: number;
}

const useLocalStyles = () => StyleSheet.create({
    container: {
        maxWidth: '100%', flexDirection: 'row', alignItems: 'center'
    },
    sideDelete: {
        backgroundColor: '#E25D5D',
        justifyContent: 'center',
        padding: 20,
        marginTop: 10,
        borderRadius: 10,
        marginLeft: 5
    }
})

const PassageComponent = ({userPassage, itemId, collectionId}: PassageProps) => {
    const passage: Passage = {
        reference: userPassage.passage.reference,
        verses: userPassage.passage.verses
    }

    const theme = useAppTheme();
    const drag = useReorderableDrag();
    const isActive = useIsActive();
    const styles = useLocalStyles();
    const { removeItemFromCollection } = useCollectionsStore();

    const RightActions = () => (
        <TouchableOpacity
            style={styles.sideDelete}
            onPress={() => {
                removeItemFromCollection(collectionId, itemId);
            }}
        >
            <Trash size={25} color={theme.colors.background} />
        </TouchableOpacity>
    );

    return (
        <Swipeable renderRightActions={() => <RightActions />}>
            <View style={styles.container}>
                <PassageContent
                    passage={passage}
                    userPassageId={userPassage.id ?? itemId}
                    onLongPress={drag}
                    disabled={isActive}
                />
            </View>
        </Swipeable>
    )
}

export default PassageComponent;