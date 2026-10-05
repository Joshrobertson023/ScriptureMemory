import { NavigationContext } from "@react-navigation/native";
import { NativeStackNavigationProp } from "@react-navigation/native-stack";
import { Clock, EllipsisVertical, List } from "lucide-react-native";
import React, { useContext, useMemo } from "react";
import { StyleSheet, Text, TouchableHighlight, TouchableOpacity, View } from "react-native";
import { useReorderableDrag } from "react-native-reorderable-list";
import { Collection } from "../../../types/collection/collection";
import { RootStackParamList } from "../../../types/router";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme, { darkTheme, lightTheme, useIsDarkMode } from "../../theme";

interface CollecitonCardProps {
    collection: Collection;
    drag?: () => void;
    onPress?: (collection: Collection) => void;
}

type AppTheme = typeof lightTheme;

function createCardStyles(theme: AppTheme) {
    return StyleSheet.create({
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
            color: theme.colors.onBackgroundSuperSoft,
            marginBottom: -3
        },
        visibility: {
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center'
        },
        menuButton: {
            position: 'absolute',
            top: 6,
            right: 6,
            padding: 8
        }
    });
}

const lightCardStyles = createCardStyles(lightTheme);
const darkCardStyles = createCardStyles(darkTheme);

export const CollectionCard = React.memo(({collection, drag, onPress}: CollecitonCardProps) => {
    const navigation = useContext(NavigationContext) as NativeStackNavigationProp<RootStackParamList> | null;
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const styles = useIsDarkMode() ? darkCardStyles : lightCardStyles;
    const { setCollectionMenuBottomSheet, setCollectionMenuSheetOpen } = useBottomSheetsStore.getState();

    const totalPassages = useMemo(
        () => collection.passageCount ?? collection.items.filter((item) => item.type === 'passage').length,
        [collection]
    );
    const totalOverdue = 0;
    const visibility = collection.visibility;

    return (
        <TouchableHighlight onLongPress={drag} delayLongPress={150} style={styles.highlight}
            onPress={drag ? undefined : () => {
                if (onPress) {
                    onPress(collection);
                    return;
                }
                navigation?.navigate('collection', { id: collection.id })
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
                        <Text style={[globalStyles.p3, styles.visibilityText]}>
                            {visibility}
                        </Text>
                    </View>
                </View>

                <TouchableOpacity
                    style={styles.menuButton}
                    hitSlop={{ top: 8, bottom: 8, left: 8, right: 8 }}
                    disabled={!!drag}
                    onPress={() => {
                        setCollectionMenuBottomSheet(collection);
                        setCollectionMenuSheetOpen(true);
                    }}
                >
                    <EllipsisVertical size={18} color={theme.colors.onBackgroundSoft} />
                </TouchableOpacity>
            </View>
        </TouchableHighlight>
    )
})

export const ReorderableCollectionCard = ({ collection, reordering }: { collection: Collection; reordering: boolean }) => {
    const drag = useReorderableDrag();

    return (
        <CollectionCard
            collection={collection}
            drag={reordering ? drag : undefined}
        />
    );
};