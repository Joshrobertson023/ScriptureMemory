import { Text, View } from "react-native";
import { Note } from "../../../types/note";
import useGlobalStyles from "../../styles/gobalStyles";

interface CollectionNoteProps {
    note: Note;
}

const CollectionNote = ({ note }: CollectionNoteProps) => {
    const styles = useGlobalStyles();

    return (
        <View style={{ width: '100%', paddingVertical: 8 }}>
            <Text style={styles.p3}>Note</Text>
            <Text style={styles.p4}>{note.text}</Text>
        </View>
    );
};

export default CollectionNote;
