import { StyleSheet, TouchableOpacity, View } from "react-native";
import { Passage } from "../../../types/passages/passage";
import AddPassageContent from "./addPassageContent";
import { UserPassage } from "../../../types/passages/userPassage";
import { useIsActive, useReorderableDrag } from "react-native-reorderable-list";
import { GripVertical, Trash2 } from "lucide-react-native";
import useAppTheme from "../../theme";
import { useCollectionsStore } from "../../stores/collections.store";

interface NewCollectionPassageProps {
    userPassage: UserPassage;
    itemId: number;
}

const useLocalStyles = () => StyleSheet.create({
    container: {
        maxWidth: '100%', flexDirection: 'row', alignItems: 'center', gap: 10
    },
    icons: {
        justifyContent: 'flex-start', alignItems: 'flex-start', height: '100%', width: 50, gap: 10
    }
})

const NewCollectionPassage = ({userPassage, itemId}: NewCollectionPassageProps) => {
    const passage: Passage = {
        reference: userPassage.passage.reference,
        verses: userPassage.passage.verses
    }

    const theme = useAppTheme();
    const drag = useReorderableDrag();
    const isActive = useIsActive();
    const styles = useLocalStyles();

    const removeItemFromNewCollection = useCollectionsStore().removeItemFromNewCollection;

    return (
        <View style={[styles.container]}>
            <AddPassageContent passage={passage} maxWidth={'90%'} />

            <View style={[styles.icons]}>
                <TouchableOpacity onLongPress={drag} disabled={isActive}>
                    <GripVertical size={20} color={theme.colors.onBackground} />
                </TouchableOpacity>
                <TouchableOpacity onPress={() => {
                    removeItemFromNewCollection(itemId);
                    console.log(itemId)}}>
                    <Trash2 size={20} color={theme.colors.onBackground} />
                </TouchableOpacity>
            </View>
        </View>
    )
}

export default NewCollectionPassage;