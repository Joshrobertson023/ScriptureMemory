import 'react-native-gesture-handler'; // MUST be at the very top


import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import * as React from 'react';
import { useEffect, useState } from 'react';

import * as SplashScreen from 'expo-splash-screen';
import * as SystemUI from 'expo-system-ui';
import { InteractionManager, StatusBar } from 'react-native';
import { GestureHandlerRootView } from 'react-native-gesture-handler';

import useAppTheme from './theme';

import { TrueSheet } from '@lodev09/react-native-true-sheet';
import { migrate } from 'drizzle-orm/expo-sqlite/migrator';
import * as Crypto from 'expo-crypto';
import { RootStackParamList } from '../types/router';
import CollectionMenuBottomSheet from './components/bottom-sheets/collectionMenuBottomSheet';
import PassageBottomSheet from './components/bottom-sheets/passageBottomSheet';
import PassageMenuBottomSheet from './components/bottom-sheets/passageMenuBottomSheet';
import SyncBottomSheet from './components/bottom-sheets/syncBottomSheet';
import SaveToCollectionDialog from './components/dialogs/saveToCollectionDialog';
import ViewNotesDialog from './components/dialogs/viewNotesDialog';
import { BottomTabWrapper } from './components/bottomTabWrapper';
import { db } from './database/client';
import { userPreferencesTable, usersTable } from './database/schema';
import migrationsBundle from './drizzle/migrations';
import { navigationRef } from './navigation';
import TabsNavigator from './screens/(tabs)/TabsNavigator';
import ReadScreen from './screens/bible/read.screen';
import CollectionScreen from './screens/collections/collection';
import { CreateCollectionScreen } from './screens/collections/createNew.screen';
import EditCollectionScreen from './screens/collections/editCollection.screen';
import { CreditsScreen } from './screens/credits';
import PracticeSessionScreen from './screens/practiceSession/practiceSession.screen';
import { useBottomSheetsStore } from './stores/bottomSheets.store';
import { useUserStore } from './stores/user.store';
import { useCustomFonts } from './styles/fonts';
import useStyles from './styles/gobalStyles';

SplashScreen.preventAutoHideAsync().catch(() => {});

const Stack = createNativeStackNavigator<RootStackParamList>();

// ─── Root component ───────────────────────────────────────────────────────────
export default function AppShell() {
  const theme = useAppTheme();
  const styles = useStyles();
  const fontsLoaded = useCustomFonts();

  const [appIsReady, setAppIsReady] = useState(false);

  //const { data: vod, isFetched: vodLoaded } = useVod();
  //const {setVod} = useAppStore();

  const passageSheet = React.useRef<TrueSheet>(null);
  const noteSheet = React.useRef<TrueSheet>(null);
  const syncSheet = React.useRef<TrueSheet>(null);
  const passageSheetOpen = useBottomSheetsStore((state) => state.passageSheetOpen);
  const syncSheetOpen = useBottomSheetsStore((state) => state.syncSheetOpen);

  const [migrationsError, setMigrationsError] = useState(false);
  const setUserId = useUserStore((state) => state.setUserId);


  async function runStartup() {
    // try {
    //   const session: Session = {
    //     deviceName: Device.deviceName || '',
    //     model: Device.modelId,
    //   }
    //   if (authStore.refreshToken) { // If refresh token, automatically login (only users who created an account have a refresh token)
    //     console.log('logging in with token')
    //     await loginUserWithToken(session);
    //   } else if (authStore.session.deviceId) {
    //     // Get new jwt if internet connection
    //     console.log('requesting new jwt')
    //   } else if (!authStore.session.deviceId) {
    //     // New user
    //     console.log('creating new user')
    //     await createUser(session);
    //   }
    // } catch (Error) {
    //   console.error(Error);
    // }

    try {
      console.log("Running db migrations")
      await migrate(db, migrationsBundle);
      console.log("Db migrations completed")

      const existingUserId = await db.select().from(usersTable);
      console.log("Existing user IDs:", existingUserId);

      if (existingUserId.length === 0) {
        console.log("No user found, creating new user")

        const newUserId = Crypto.randomUUID();

        setUserId(newUserId);
        await db.insert(usersTable).values({ userId: newUserId }).onConflictDoNothing();
        await db.insert(userPreferencesTable)
          .values({ userId: newUserId, preferredBibleVersion: 'kjv' })
          .onConflictDoNothing();
      }

    } catch (error) {
      setMigrationsError(true);
      console.error("Failed to initialize database", error);
      return;
    }

    setAppIsReady(true);
  }
  
  useEffect(() => {
      runStartup();
    }, []);
    
    useEffect(() => {
      const task = InteractionManager.runAfterInteractions(() => {
        SystemUI.setBackgroundColorAsync(theme.colors.background).catch((e) =>
          console.warn('Failed to set system UI background:', e),
        );
      });
      return () => task.cancel();
    }, [theme.colors.background]);
  
  
    // ── Hide splash screen once ready ────────────────────────────────────────
    useEffect(() => {
      if (!appIsReady) {
        console.log("App is not ready")
        return;
      }

      if (migrationsError) {
        console.log("No migrations error")
        return;
      }

      if (!fontsLoaded) {
        console.log("Fonts not loaded")
        return;
      }

      // if (!vodLoaded)
      //   return;

      console.log("App is ready, hiding splash");

      SplashScreen.hideAsync().catch(() => {});
    }, [appIsReady, fontsLoaded, migrationsError]);

    // set passage bottom sheet ref
    useEffect(() => {
      if (passageSheetOpen) {
        passageSheet.current?.present();
      } else {
        passageSheet.current?.dismiss();
      }
    }, [passageSheetOpen]);

    // set sync bottom sheet ref
    useEffect(() => {
      if (syncSheetOpen) {
        syncSheet.current?.present();
      } else {
        syncSheet.current?.dismiss();
      }
    }, [syncSheetOpen])

  if (!appIsReady || !fontsLoaded) {
    return null;
  } 


    return (
        
      <GestureHandlerRootView style={{ flex: 1, backgroundColor: theme.colors.background }}>
        <StatusBar barStyle={theme.dark ? 'light-content' : 'dark-content'} />
        <BottomTabWrapper>
            <NavigationContainer theme={theme} ref={navigationRef}>
              <Stack.Navigator
                screenOptions={{ contentStyle: { backgroundColor: theme.colors.background } }}
              >
                
                <Stack.Screen 
                  name="(tabs)" 
                  component={TabsNavigator} 
                  options={{ headerShown: false, animation: 'none' }} 
                />
                {/* <Stack.Screen
                  name="(practice)"
                  component={PracticeLayout}
                  options={{headerShown: false, animation: 'none'}}
                /> */}
                <Stack.Screen
                  name="createCollection"
                  component={CreateCollectionScreen}
                  options={{
                    headerShown: true,
                    headerTitle: 'New Collection',
                    animation: 'default',
                    headerStyle: {
                      backgroundColor: theme.colors.background2,
                    },
                    headerTitleStyle: {
                      color: theme.colors.onBackground,
                      fontSize: 20
                    },
                    headerTintColor: theme.colors.onBackground,
                  }}
                />
                <Stack.Screen
                  name="editCollection"
                  component={EditCollectionScreen}
                  options={{
                    headerShown: true,
                    animation: 'default',
                    headerStyle: {
                      backgroundColor: theme.colors.background2,
                    },
                    headerTitleStyle: {
                      color: theme.colors.onBackground,
                      fontSize: 20
                    },
                    headerTintColor: theme.colors.onBackground,
                  }}
                />
                <Stack.Screen
                  name="collection"
                  component={CollectionScreen}
                  options={{
                    headerShown: true,
                    headerStyle: {
                      backgroundColor: theme.colors.background2,
                    },
                    headerTitleStyle: {
                      color: theme.colors.onBackground,
                      fontSize: 20
                    },
                    headerTintColor: theme.colors.onBackground,
                  }}
                />
                <Stack.Screen
                  name="read"
                  component={ReadScreen}
                  options={{ headerShown: false }}
                />
                <Stack.Screen
                  name="practiceSession"
                  component={PracticeSessionScreen}
                  options={{
                    headerShown: true,
                    headerStyle: {
                      backgroundColor: theme.colors.background2,
                    },
                    headerTintColor: theme.colors.onBackground,
                    headerShadowVisible: false,
                  }}
                />
                <Stack.Screen
                  name="credits"
                  component={CreditsScreen}
                  options={{ headerShown: false }}
                />
              </Stack.Navigator>
            </NavigationContainer>
          <PassageBottomSheet ref={passageSheet}/>
          <ViewNotesDialog />
          <SaveToCollectionDialog />
          <SyncBottomSheet />
          <CollectionMenuBottomSheet />
          <PassageMenuBottomSheet />
        </BottomTabWrapper>
      </GestureHandlerRootView>
    )
}