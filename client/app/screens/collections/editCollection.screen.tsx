import { useEffect, useMemo, useState } from "react";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";
import { Alert, StyleSheet, View } from "react-native";
import { Collection } from "../../../types/collection/collection";
import { initialCollection, useCollectionsStore } from "../../stores/collections.store";
import { useNavigation } from "@react-navigation/native";

const EditCollectionScreen = () => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const styles = useMemo(() => StyleSheet.create({

    }), [theme]);

    const navigation = useNavigation();

    useEffect(() => {
        const unsubscribe = navigation.addListener("beforeRemove", (e) => {
            e.preventDefault();

            Alert.alert(
                "Unsaved changes",
                "Do you want to save your changes?",
                [
                    {
                        "text": "Don't Save",
                        "style": 'destructive',
                        'onPress': () => navigation.dispatch(e.data.action),
                    },
                    {
                        'text': 'Cancel',
                        'style': 'cancel',
                    },
                    {
                        'text': 'Save',
                        onPress: () => setEditingCollection(collection)
                    }
                ]
            )
        });
        return unsubscribe;
    }, [navigation])

    const [collection, setCollection] = useState<Collection>(initialCollection);
    const {setEditingCollection, clearEditingCollection} = useCollectionsStore();

    return (
        <View style={globalStyles.screen}>
            
        </View>
    )
}

export default EditCollectionScreen;