import { Keyboard, TextInput, Text, TouchableHighlight, TouchableOpacity, TouchableWithoutFeedback, View } from "react-native"
import useGlobalStyles from "../../styles/gobalStyles"
import { useCollectionsStore } from "../../stores/collections.store";
import { ArrowUpDown, ChevronRight } from "lucide-react-native";
import useAppTheme from "../../theme";
import VisibilityBottomSheet from "../../components/bottom-sheets/visibilityBottomSheet";
import { useRef } from "react";
import { TrueSheet } from "@lodev09/react-native-true-sheet";
import AddPassageBottomSheet from "../../components/bottom-sheets/addPassageBottomSheet";

export const CreateCollectionScreen = () => {
    const styles = useGlobalStyles();
    const theme = useAppTheme();
    const {newCollection, setNewCollection} = useCollectionsStore();
    const visibilityBottomSheet = useRef<TrueSheet>(null);
    const addPassageBottomSheet = useRef<TrueSheet>(null);

    return (
        <TouchableWithoutFeedback onPress={Keyboard.dismiss}>
            <View style={{...styles.screen, gap: 5}}>
                <View style={{display: 'flex', flexDirection: 'row', justifyContent: 'center', alignItems: 'center', gap: 15}}>
                    <TextInput
                        value={newCollection.title}
                        onChangeText={(text) => setNewCollection({...newCollection, title: text})}
                        maxLength={20}
                        style={styles.input}
                        placeholder="New Collection Title"
                    />
                    <TouchableOpacity >
                        <ArrowUpDown size={28} color={theme.colors.onBackground} />
                    </TouchableOpacity>
                </View>

                <TouchableOpacity 
                    onPress={() => visibilityBottomSheet.current?.present()}>
                    <View style={{backgroundColor: theme.colors.elevation, paddingVertical: 10, paddingHorizontal: 25, borderRadius: 5, display: 'flex', flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', width: '100%'}}>
                        <Text style={styles.p3}>Visibility</Text>
                        <ChevronRight size={28} color={theme.colors.onBackground} />
                    </View>
                </TouchableOpacity>

                <TouchableOpacity style={styles.elevationButton} onPress={() => addPassageBottomSheet.current?.present()}>
                    <Text style={styles.p3}>Add Passage</Text>
                </TouchableOpacity>

            <VisibilityBottomSheet 
                ref={visibilityBottomSheet}
                currentVisibility={newCollection.visibility}
            />
            <AddPassageBottomSheet
                ref={addPassageBottomSheet}/>
            </View>
        </TouchableWithoutFeedback>
    )
}