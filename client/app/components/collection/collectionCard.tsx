import { StyleSheet, Text, TouchableHighlight, TouchableNativeFeedback, TouchableOpacity, View } from "react-native";
import { useCollectionsStore } from "../../stores/collections.store"
import { Collection } from "../../../types/collection/collection";
import { useMemo } from "react";
import useAppTheme from "../../theme";
import useGlobalStyles from "../../styles/gobalStyles";
import { Archive, Clock, List, Trash } from "lucide-react-native";
import { useIsActive, useReorderableDrag } from "react-native-reorderable-list";
import Swipeable from 'react-native-gesture-handler/ReanimatedSwipeable';

interface CollecitonCardProps {
    collection: Collection;
}

export const CollectionCard = ({collection}: CollecitonCardProps) => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const useLocalStyles = () => useMemo(() => StyleSheet.create({
        highlight: {
            marginTop: 10,
            borderRadius: 10
        },
        section: {
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'flex-start',
        },
        section2: {
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            width: 60,
            marginRight: -7
        },
        passagesChip: {
            backgroundColor: theme.colors.elevation2,
            borderRadius: 30,
            paddingVertical: 2,
            paddingHorizontal: 8,
            gap: 3,
            flexDirection: 'row',
            justifyContent: 'center',
            alignItems: 'center'
        },
        overdueChip: {
            backgroundColor: theme.colors.elevation2,
            borderRadius: 30,
            paddingVertical: 4,
            paddingHorizontal: 12,
            gap: 5,
            flexDirection: 'row',
            justifyContent: 'center',
            alignItems: 'center'
        },
        visibilityText: {
            ...globalStyles.p3,
            color: theme.colors.onBackgroundSuperSoft,
            marginBottom: -3
        },
        visibility: {
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center'
        },
        sideDelete: {
            backgroundColor: '#E25D5D',
            justifyContent: 'center',
            padding: 20,
            marginTop: 10,
            borderRadius: 10,
            marginLeft: 5
        },
        sideArchive: {
            backgroundColor: '#4D6CC7',
            justifyContent: 'center',
            padding: 20,
            marginTop: 10,
            borderRadius: 10,
            marginLeft: 5
        }
    }), [theme])
    const styles = useLocalStyles();
    const drag = useReorderableDrag();
    const isActive = useIsActive();

    const totalPassages = (collection.items.map((item) => item.type === 'passage')).reduce((prev, next) => prev + 1, 0);
    const totalOverdue = 0;
    let visibility = '';
    switch (collection.visibility) {
        case 0: 
            visibility = 'Private'
            break;
        case 1:
            visibility = 'Friends'
            break;
        case 2: 
            visibility = 'Public'
            break;
    }

    const {deleteCollection} = useCollectionsStore();

    const RightActions = () => (
        <>
            <TouchableOpacity style={styles.sideDelete}
                onPress={() => {
                    deleteCollection(collection.id);
                }}
            >
                <Trash size={25} color={theme.colors.background} />
            </TouchableOpacity>
            <TouchableOpacity style={styles.sideArchive}
                onPress={() => {

                }}
            >
                <Archive size={25} color={theme.colors.background} />
            </TouchableOpacity>
        </>
    )

    return (
        <Swipeable renderRightActions={() => <RightActions />}>
            <TouchableHighlight onLongPress={drag} disabled={isActive} style={styles.highlight} 
                onPress={() => {
                    
            }}>
                <View style={globalStyles.collectionCard}>
                    <View style={styles.section}>
                        
                        <Text style={globalStyles.collectionCardTitle}>{collection.title}</Text>
                        <View style={styles.passagesChip}>
                            <List size={12} color={theme.colors.onBackground} />
                            <Text style={globalStyles.p4}>{totalPassages}</Text>
                        </View>

                    </View>
                    <View style={styles.section2}>

                        {totalOverdue > 0 ? (
                            <View style={styles.overdueChip}>
                                <Clock size={16} color={theme.colors.onBackground} />
                                <Text style={globalStyles.p3}>{totalOverdue}</Text>
                            </View>
                        ) : (
                            <View />
                        )}

                        <View style={styles.visibility}>
                            <Text style={styles.visibilityText}>
                                {visibility}
                            </Text>
                        </View>
                    </View>
                </View>
            </TouchableHighlight>
        </Swipeable>
    )
}