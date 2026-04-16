import { Text, TouchableHighlight, View } from "react-native";
import { Passage } from "../../../types/passages/passage";
import useGlobalStyles from "../../styles/gobalStyles";
import { useCollectionsStore } from "../../stores/collections.store";

interface AddPassageProps {
    passage: Passage;
}

const AddPassage = ({passage}: AddPassageProps) => {
    const styles = useGlobalStyles();
    const {addPassageToNewCollection, removePassageFromNewCollection, newCollection} = useCollectionsStore();

    const passageSaved = newCollection.passages.some((p) => p.reference.readableReference === passage.reference.readableReference);

    return (
        <View style={{display: 'flex', justifyContent: 'flex-start', alignItems: 'center'}}>
            <Text style={styles.p2}>{passage.reference.readableReference}</Text>
            <View>
                {passage.verses.map((verse, index) => (
                    <Text style={styles.p3}>
                        {passage.verses.length > 1 && (verse.reference.verses.at(0))}: {verse.text}
                    </Text>
                ))}
            </View>

            {passageSaved ? (
                <TouchableHighlight onPress={() => {
                        removePassageFromNewCollection(passage);
                    }}>
                    <View style={styles.outlineButtonSkinny}>
                        <Text style={styles.p2}>Remove</Text>
                    </View>
                </TouchableHighlight>
            ) : (
                <TouchableHighlight onPress={() => {
                        addPassageToNewCollection(passage);
                    }}>
                    <View style={styles.outlineButtonSkinny}>
                        <Text style={styles.p2}>Add</Text>
                    </View>
                </TouchableHighlight>
            )}
        </View>
    )
}

export default AddPassage;