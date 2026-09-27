import Ionicons from '@expo/vector-icons/Ionicons';
import { BottomTabBar, BottomTabBarProps, createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import * as SystemUI from 'expo-system-ui';
import React, { useContext, useEffect, useState } from 'react';
import { Text, View } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import useAppTheme from '../../theme';

import Animated, { useAnimatedStyle, useSharedValue } from 'react-native-reanimated';
import { TabBarVisibilityContext } from '../../components/bottomTabWrapper';
import { useAppStore } from '../../stores/appState.store';
import { useBottomSheetsStore } from '../../stores/bottomSheets.store';
import useGlobalStyles from '../../styles/gobalStyles';
import { BibleScreen } from './bible.screen';
import { CollectionsScreen } from './collections.screen';
import { HomeScreen } from './home.screen';
import { PracticeScreen } from './practice.screen';
import { ProfileScreen } from './profile.screen';

const Tab = createBottomTabNavigator();

export default function TabLayout() {
  const theme = useAppTheme();
  const styles = useGlobalStyles();
  const insets = useSafeAreaInsets();
  const inactiveColor = theme.colors.elevation3;

  const [isProfileDrawerOpen, setIsProfileDrawerOpen] = useState(false);
  const syncStatus = useAppStore((state) => state.syncStatus);
  const setSyncSheetOpen = useBottomSheetsStore((state) => state.setSyncSheetOpen);

  useEffect(() => {
    SystemUI.setBackgroundColorAsync(theme.colors.background).catch(() => {});
  }, [theme.colors.background]);

  const context = useContext(TabBarVisibilityContext);
  const fallbackTranslateY = useSharedValue(0);
  const tabBarTranslateY = context?.tabBarTranslateY ?? fallbackTranslateY;
  const tabBarAnimatedStyle = useAnimatedStyle(
    () => ({
      transform: [{ translateY: tabBarTranslateY.value }],
    }),
    [tabBarTranslateY]
  );

  return (
        <Tab.Navigator
          tabBar={(props: BottomTabBarProps) => (
            <Animated.View style={tabBarAnimatedStyle}>
              <BottomTabBar {...props} />
            </Animated.View>
          )}
          screenOptions={{
            animation: 'fade',
            tabBarActiveTintColor: theme.colors.onBackground,
            tabBarInactiveTintColor: theme.colors.inactiveTab,
            tabBarLabelPosition: 'below-icon',
            tabBarStyle: {
              position: 'absolute',
              bottom: 0,
              left: 0,
              right: 0,
              backgroundColor: theme.colors.background,
              height: 70 + insets.bottom + 10,
              paddingBottom: Math.max(insets.bottom, 10) + 10,
              paddingTop: 10,
              paddingLeft: 5,
              paddingRight: 5,
              borderTopColor: theme.colors.elevation2,
              borderTopWidth: 0.2,
            },
            tabBarItemStyle: {
              alignItems: 'center',
              justifyContent: 'center',
              paddingHorizontal: 0,
              paddingVertical: 6,
              width: '100%',
            },
            tabBarIconStyle: {
              width: '100%',
              alignItems: 'center',
              justifyContent: 'center',
              marginBottom: 0,
              marginLeft: 2,
            },
            tabBarLabelStyle: {
              width: '100%',
              textAlign: 'center',
              marginTop: 0,
            },
            headerShown: true,
            headerStyle: {
              backgroundColor: theme.colors.background2,
              borderBottomWidth: 0,
              borderBottomColor: 'transparent',
            },
            headerTitleStyle: {
              color: theme.colors.onBackground,
            },
            headerTintColor: theme.colors.onBackground,
          }}
        >
          {/* ── Collections ── */}
          <Tab.Screen
            name="Collections"
            component={CollectionsScreen}
            options={{
              headerShown: true,
              headerTitle: 'Collections',
              tabBarIcon: ({ focused }) => (
                <View style={{ position: 'relative' }}>
                  <Ionicons
                    name={focused ? 'albums' : 'albums-outline'}
                    color={focused ? theme.colors.onBackground : inactiveColor}
                    size={28}
                  />
                </View>
              ),
              tabBarLabel: ({ focused }) => (
                <Text style={{ fontSize: 14, fontWeight: '600', color: focused ? theme.colors.onBackground : inactiveColor, textAlign: 'center' }}>
                  Collections
                </Text>
              ),
              // headerRight: () => (
              //   <TouchableOpacity style={{marginLeft: -30}} onPress={() => { setSyncSheetOpen(true) }}>
              //           {syncStatus === 'Synced' ? (
              //               <CloudCheck size={28} color={theme.colors.onBackground} />
              //           ) : syncStatus === 'Syncing' ? (
              //               <CloudSync size={28} color={theme.colors.onBackground} />
              //           ) : (
              //               <CloudAlert size={28} color={theme.colors.onBackground} />
              //           )}
              //       </TouchableOpacity>
              // ),
              headerSearchBarOptions: {
                placeholder: "Search Collections...",
              }
            }}
          />

          {/* ── Practice ── */}
          <Tab.Screen
            name="Practice"
            component={PracticeScreen}
            options={{
              headerShown: true,
              tabBarIcon: ({ focused }) => (
                <Ionicons
                  name={focused ? 'barbell' : 'barbell-outline'}
                  color={focused ? theme.colors.onBackground : inactiveColor}
                  size={28}
                />
              ),
              tabBarLabel: ({ focused }) => (
                <Text style={{ fontSize: 14, fontWeight: '600', color: focused ? theme.colors.onBackground : inactiveColor, textAlign: 'center' }}>
                  Practice
                </Text>
              ),
            }}
          />

          {/* ── Search (center FAB-style) ── */}
          <Tab.Screen
            name="Search"
            component={HomeScreen}
            options={{
              headerShown: false,
              tabBarIcon: ({ focused }) => (
                <View style={{
                  zIndex: 1000000,
                  height: 67,
                  width: 67,
                  padding: 10,
                  borderRadius: 100,
                  marginBottom: -10,
                  backgroundColor: focused ? theme.colors.elevation : theme.colors.background,
                }}>
                  <Ionicons name="search-outline" color={focused ? theme.colors.onBackground : inactiveColor} size={45} />
                </View>
              ),
              tabBarLabel: ({ focused }) => (
                <Text style={{
                  fontSize: 14,
                  fontWeight: focused ? '800' : '400',
                  color: 'transparent',
                  textAlign: 'center',
                  position: 'absolute',
                  zIndex: 0,
                }}>
                  Search
                </Text>
              ),
            }}
          />

          {/* ── Bible ── */}
          <Tab.Screen
            name="Bible"
            component={BibleScreen}
            options={{
              headerShown: false,
              tabBarIcon: ({ focused }) => (
                <Ionicons
                  name={focused ? 'book-sharp' : 'book-outline'}
                  color={focused ? theme.colors.onBackground : inactiveColor}
                  size={28}
                />
              ),
              tabBarLabel: ({ focused }) => (
                <Text style={{ fontSize: 14, fontWeight: '600', color: focused ? theme.colors.onBackground : inactiveColor, textAlign: 'center' }}>
                  Bible
                </Text>
              ),
            }}
          />

          {/* ── Explore ── */}
          <Tab.Screen
            name="Profile"
            component={ProfileScreen}
            options={{
              headerShown: false,
              tabBarIcon: ({ focused }) => (
                <Ionicons
                  name={focused ? 'person' : 'person-outline'}
                  color={focused ? theme.colors.onBackground : inactiveColor}
                  size={28}
                />
              ),
              tabBarLabel: ({ focused }) => (
                <Text style={{ fontSize: 14, fontWeight: '600', color: focused ? theme.colors.onBackground : inactiveColor, textAlign: 'center' }}>
                  Profile
                </Text>
              ),
            }}
          />
        </Tab.Navigator>
  );
}