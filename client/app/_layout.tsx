import 'react-native-gesture-handler'; // MUST be at the very top

import Ionicons from '@expo/vector-icons/Ionicons';
import AsyncStorage from '@react-native-async-storage/async-storage';

import * as React from 'react';
import {createStaticNavigation} from '@react-navigation/native';
import {createNativeStackNavigator} from '@react-navigation/native-stack';

import * as SecureStore from 'expo-secure-store';
import * as SplashScreen from 'expo-splash-screen';
import * as SystemUI from 'expo-system-ui';
import { useEffect, useRef, useState } from 'react';
import { Image, Platform, TouchableOpacity, View } from 'react-native';
import { GestureHandlerRootView } from 'react-native-gesture-handler';
import { PaperProvider, Portal } from 'react-native-paper';

import useStyles from '../styles';
import useAppTheme from '../theme';

SplashScreen.preventAutoHideAsync().catch(() => {});


// ─── Helpers ──────────────────────────────────────────────────────────────────

async function clearAuthData() {
  await Promise.allSettled([
    SecureStore.deleteItemAsync('userToken'),
    AsyncStorage.removeItem(RECENT_SEARCHES_KEY),
  ]);
}

// ─── Startup ────────────────────────────

async function runStartup(
  setUser: (u: any) => void,
) {
  let authToken: string | null = null;

  try {
    authToken = await SecureStore.getItemAsync('userToken');
  } catch (e) {
    console.error('SecureStore read failed:', e);
  }

  if (authToken) {
    const fetchedUser = await loginUserWithToken(token);

    // let streakLength = 0;
    // try {
    //   streakLength = await getStreakLength(fetchedUser.username);
    // } catch (e) {
    //   console.error('Failed to fetch streak length:', e);
    // }
  }

  // Always load verse of day and searches regardless of auth
  const [verseResult, searchesResult] = await Promise.allSettled([
    // getCurrentVerseOfDayAsUserVerse(),
    // getPopularSearches(10),
  ]);
}

// ─── Root component ────────────────────────────────────────────────────────────────

export default function RootLayout() {
  const theme = useAppTheme();
  const styles = useStyles();

  const [appIsReady, setAppIsReady] = useState(false);

  const setUser = useAppStore((s) => s.setUser);
  const user = useAppStore((s) => s.user);
  const getHomePageStats = useAppStore((s) => s.getHomePageStats);
  const setCollections = useAppStore((s) => s.setCollections);
  const setPopularSearches = useAppStore((s) => s.setPopularSearches);
  const setVerseOfDay = useAppStore((s) => s.setVerseOfDay);
  const { openSettingsSheet } = useAppStore((s) => s.collectionsSheetControls);

  const notificationListener = useRef<Notifications.Subscription | null>(null);
  const responseListener = useRef<Notifications.Subscription | null>(null);

  // ── System UI background ────────────────────────────────────────────────────
  useEffect(() => {
    SystemUI.setBackgroundColorAsync(theme.colors.background).catch((e) =>
      console.warn('Failed to set system UI background:', e),
    );
  }, [theme.colors.background]);

  // ── Update last seen every minute ─────────────────────────────────────────────────────
  // useEffect(() => {
  //   if (!user.username || user.username === 'Default User') return;

  //   updateLastSeen(user.username).catch((e) => console.error('updateLastSeen failed:', e));

  //   const interval = setInterval(() => {
  //     updateLastSeen(user.username).catch((e) => console.error('updateLastSeen failed:', e));
  //   }, 60_000);

  //   return () => clearInterval(interval);
  // }, [user.username]);

  // ── App startup ─────────────────────────────────────
  useEffect(() => {
    const TIMEOUT_MS = 4000;

    const controller = new AbortController();
    let didTimeout = false;
    const timeoutId = setTimeout(() => {
      didTimeout = true;
      controller.abort();
    }, TIMEOUT_MS);

    runStartup(setUser, setCollections, setVerseOfDay, setPopularSearches, getHomePageStats)
      .catch(async (e) => {
        if (!didTimeout) console.warn('Startup error:', e);
        await clearAuthData();
      })
      .finally(() => {
        clearTimeout(timeoutId);
        setAppIsReady(true);
      });
  }, []);

  // Hide splash screen once ready
  useEffect(() => {
    if (appIsReady) SplashScreen.hideAsync().catch(() => {});
  }, [appIsReady]);




  // ── Main navigator ──────────────────────────────────────────────────────────


  const RootStack = createNativeStackNavigator({
    screens: {
      Home: {
        screen: HomeScreen,
        options: {
          title: 'Home',
          ...sharedHeaderStyle,
        }
      }
    }
  })

  const Navigation = createStaticNavigation(RootStack);
  
  const sharedHeaderStyle = {
    headerStyle: { backgroundColor: theme.colors.background },
    headerTitleStyle: { color: theme.colors.onBackground },
    headerTintColor: theme.colors.onBackground,
  } as const;

  const sharedHeaderStyleNoShadow = {
    ...sharedHeaderStyle,
    headerShadowVisible: false,
  } as const;



  return <Navigation />;




  // return (
  //   <GestureHandlerRootView style={{ flex: 1, backgroundColor: theme.colors.background }}>
  //     <PaperProvider theme={theme}>
  //       <Portal.Host>
  //         <Stack screenOptions={{ contentStyle: { backgroundColor: theme.colors.background } }}>
  //           <Stack.Screen name="(tabs)" options={{ headerShown: false, animation: 'fade' }} />
  //           <Stack.Screen name="(auth)" options={{ headerShown: false }} />

  //           <Stack.Screen name="privacy"    options={{ title: 'Privacy Policy',               ...sharedHeaderStyle }} />
  //           <Stack.Screen name="terms"      options={{ title: 'Terms of Service',             ...sharedHeaderStyle }} />
  //           <Stack.Screen name="activity"   options={{ title: 'Activity Tracking & Sharing',  ...sharedHeaderStyle }} />
  //           <Stack.Screen name="about"      options={{ title: 'About',                        ...sharedHeaderStyle }} />
  //           <Stack.Screen name="notifications" options={{ title: 'Notifications',             ...sharedHeaderStyle }} />
  //           <Stack.Screen name="admin"      options={{ title: 'Admin Panel',                  ...sharedHeaderStyle }} />
  //           <Stack.Screen name="settings"   options={{ title: 'Settings',                     ...sharedHeaderStyleNoShadow }} />
  //           <Stack.Screen name="practiceSession" options={{ title: 'Practice',                ...sharedHeaderStyle }} />

  //           <Stack.Screen name="push-notifications-tutorial" options={{ title: 'Push Notifications', ...sharedHeaderStyleNoShadow }} />

  //           <Stack.Screen name="user"     options={{ headerShown: false }} />
  //           <Stack.Screen name="book"     options={{ headerShown: false }} />
  //           <Stack.Screen name="chapters" options={{ headerShown: false }} />

  //           <Stack.Screen name="collections/addnew"                options={{ title: 'New Collection',        ...sharedHeaderStyleNoShadow }} />
  //           <Stack.Screen name="collections/reorderCollections"    options={{ title: 'Reorder Collections',   ...sharedHeaderStyleNoShadow }} />
  //           <Stack.Screen name="collections/reorderVerses"         options={{ title: 'Reorder Passages',      ...sharedHeaderStyleNoShadow }} />
  //           <Stack.Screen name="collections/reorderExistingVerses" options={{ title: 'Reorder Passages',      ...sharedHeaderStyleNoShadow }} />
  //           <Stack.Screen name="collections/editCollection"        options={{ title: 'Edit Collection',       ...sharedHeaderStyleNoShadow }} />
  //           <Stack.Screen name="collections/publishCollection"     options={{ title: 'Publish Collection',    ...sharedHeaderStyleNoShadow }} />

  //           <Stack.Screen
  //             name="collections/[id]"
  //             options={{
  //               title: '',
  //               ...sharedHeaderStyleNoShadow,
  //               headerRight: () => (
  //                 <View style={{ flexDirection: 'row', gap: 15, marginRight: 10 }}>
  //                   <TouchableOpacity onPress={openSettingsSheet}>
  //                     <Ionicons style={{ marginTop: 4 }} name="ellipsis-vertical" size={32} color={theme.colors.onBackground} />
  //                   </TouchableOpacity>
  //                 </View>
  //               ),
  //             }}
  //           />

  //           <Stack.Screen
  //             name="explore/collection/[id]"
  //             options={{ title: '', ...sharedHeaderStyle }}
  //           />
  //         </Stack>
  //       </Portal.Host>
  //     </PaperProvider>
  //   </GestureHandlerRootView>
  // );
}