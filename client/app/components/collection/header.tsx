import React, { useRef } from "react"
import { TouchableOpacity, View } from "react-native"
import Ionicons from '@expo/vector-icons/Ionicons';
import useAppTheme from "../../theme";
import { useNavigation } from "@react-navigation/native";
import { NativeStackNavigationProp } from "@react-navigation/native-stack";
import { RootStackParamList } from "../../../types/router";
import { TrueSheet } from "@lodev09/react-native-true-sheet";
import CollectionsSortBottomSheet from "../bottom-sheets/collectionsSortBottomSheet";
import { useUserStore } from "../../stores/user.store";
import { ArrowUpDown, CloudAlert, CloudCheck, CloudSync, Plus, Search } from "lucide-react-native";
import SyncBottomSheet from "../bottom-sheets/syncBottomSheet";

export const CollectionsPageHeader = () => {
    const theme = useAppTheme();
    type NavigationProp = NativeStackNavigationProp<RootStackParamList>;
    const navigation = useNavigation<NavigationProp>();
    const sortSheet = useRef<TrueSheet>(null);
    const syncSheet = useRef<TrueSheet>(null);
    const collectionsSort = useUserStore().user.preferences.collectionsSort;

    const syncSuccessful = false;
    const syncing = false;
    const syncStatus = 0;

    return (
        <View style={{width: '100%', display: 'flex', justifyContent: 'space-between', flexDirection: 'row'}}>
            <TouchableOpacity onPress={() => navigation.navigate('createCollection')}>
                <Plus size={28} color={theme.colors.onBackground} />
            </TouchableOpacity>
            <View style={{display: 'flex', flexDirection: 'row', justifyContent: 'center', alignItems: 'center', gap: 15}}>
                <TouchableOpacity onPress={() => { syncSheet.current?.present() }} >
                {syncSuccessful ? (
                    <CloudCheck size={28} color={theme.colors.onBackground} />
                ) : (
                    syncing ? (
                        <CloudSync size={28} color={theme.colors.onBackground} />
                    ) : (
                        <CloudAlert size={28} color={theme.colors.onBackground} />
                    )
                )}
                </TouchableOpacity>
                <TouchableOpacity onPress={() => {}}>
                    <Search size={28} color={theme.colors.onBackground} />
                </TouchableOpacity>
                <TouchableOpacity onPress={() => {sortSheet.current?.present()}}>
                    <ArrowUpDown size={28} color={theme.colors.onBackground} />
                </TouchableOpacity>
            
            </View>
            <SyncBottomSheet ref={syncSheet} syncStatus={syncStatus} />
            <CollectionsSortBottomSheet ref={sortSheet} currentOrder={collectionsSort}/>
        </View>
    )
}