import { TouchableOpacity, View } from "react-native";
import { Passage } from "../../../types/passages/passage";
import PassageContent from "./passageContent";
import { UserPassage } from "../../../types/passages/userPassage";
import { useIsActive, useReorderableDrag } from "react-native-reorderable-list";
import { GripVertical, Trash2 } from "lucide-react-native";
import useAppTheme from "../../theme";
import { useCollectionsStore } from "../../stores/collections.store";

interface NewCollectionPassageProps {
    userPassage: UserPassage;
    itemId: number;
}

const NewCollectionPassage = ({userPassage, itemId}: NewCollectionPassageProps) => {
    const passage: Passage = {
        reference: userPassage.passage.reference,
        verses: userPassage.passage.verses
    }

    const theme = useAppTheme();
    const drag = useReorderableDrag();
    const isActive = useIsActive();

    const removeItemFromNewCollection = useCollectionsStore().removeItemFromNewCollection;

    return (
        <View style={{maxWidth: '100%', flexDirection: 'row', alignItems: 'center', gap: 10}}>
            <PassageContent passage={passage} />

            <View style={{justifyContent: 'space-between', alignItems: 'stretch'}}>
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