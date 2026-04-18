import { TouchableOpacity, View } from "react-native";
import { Passage } from "../../../types/passages/passage";
import PassageContent from "./passageContent";
import { UserPassage } from "../../../types/passages/userPassage";
import { useIsActive, useReorderableDrag } from "react-native-reorderable-list";
import { GripVertical } from "lucide-react-native";
import useAppTheme from "../../theme";

interface NewCollectionPassageProps {
    userPassage: UserPassage;
}

const NewCollectionPassage = ({userPassage}: NewCollectionPassageProps) => {
    const passage: Passage = {
        reference: userPassage.passage.reference,
        verses: userPassage.passage.verses
    }

    const theme = useAppTheme();
    const drag = useReorderableDrag();
    const isActive = useIsActive();

    return (
        <View style={{flexDirection: 'row', alignItems: 'center'}}>
            <TouchableOpacity onLongPress={drag} disabled={isActive}>
                <GripVertical size={20} color={theme.colors.onBackground} />
            </TouchableOpacity>

            <TouchableOpacity>
                <PassageContent passage={passage} />
            </TouchableOpacity>
        </View>
    )
}

export default NewCollectionPassage;