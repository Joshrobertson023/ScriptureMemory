import { Text, TouchableHighlight, View } from "react-native";
import { Passage } from "../../../types/passages/passage";
import useGlobalStyles from "../../styles/gobalStyles";

interface AddPassageProps {
    passage: Passage;
}

const AddPassage = ({passage}: AddPassageProps) => {
    const styles = useGlobalStyles();

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

            <TouchableHighlight>
                <View style={styles.outlineButtonSkinny}>
                    <Text style={styles.p2}>Add</Text>
                </View>
            </TouchableHighlight>
        </View>
    )
}