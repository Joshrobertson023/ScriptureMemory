import Ionicons from '@expo/vector-icons/Ionicons';
import { BottomTabBar, BottomTabBarProps, createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { NavigationContainer } from '@react-navigation/native';
import * as SystemUI from 'expo-system-ui';
import React, { useState } from 'react';
import { Pressable, Text, TouchableOpacity, View } from 'react-native';
import { Drawer } from 'react-native-drawer-layout';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import useAppTheme from '../../theme';

// Import your screen components
import {BibleScreen} from './bible.screen';
import {ExploreScreen} from './profile.screen';
import {HomeScreen} from './home.screen';
import { CollectionsScreen } from './collections.screen';
import useGlobalStyles from '../../styles/gobalStyles';
import { Users } from 'lucide-react-native';

const Tab = createBottomTabNavigator();

export default function TabLayout() {
  const theme = useAppTheme();
  const styles = useGlobalStyles();
  const insets = useSafeAreaInsets();
  const inactiveColor = theme.colors.elevation3;

  // Replace these with your actual state/store values
  const user = { isPaid: false };
  const numNotifications = 0;
  const overdueCount = 0;

  const [isProfileDrawerOpen, setIsProfileDrawerOpen] = useState(false);

  SystemUI.setBackgroundColorAsync(theme.colors.background);

  return (
      <Drawer
        style={{ flex: 1, backgroundColor: theme.colors.background }}
        open={isProfileDrawerOpen}
        onOpen={() => setIsProfileDrawerOpen(true)}
        onClose={() => setIsProfileDrawerOpen(false)}
        drawerPosition="right"
        drawerType="front"
        overlayStyle={{ backgroundColor: 'rgba(0, 0, 0, 0.4)' }}
        drawerStyle={{ width: '80%', backgroundColor: theme.colors.background }}
        renderDrawerContent={() => <View />}
      >
        <Tab.Navigator
          tabBar={(props: BottomTabBarProps) => <BottomTabBar {...props} />}
          screenOptions={{
            animation: 'fade',
            tabBarActiveTintColor: theme.colors.onBackground,
            tabBarInactiveTintColor: theme.colors.inactiveTab,
            tabBarLabelPosition: 'below-icon',
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
            tabBarStyle: {
              backgroundColor: theme.colors.background,
              height: 70 + insets.bottom + 10,
              paddingBottom: Math.max(insets.bottom, 10) + 10,
              paddingTop: 10,
              paddingLeft: 5,
              paddingRight: 5,
              borderTopColor: theme.colors.elevation2,
              borderTopWidth: 0.2,
            },
          }}
        >
          {/* ── Home ── */}
          <Tab.Screen
            name="KJV Bible"
            component={HomeScreen}
            options={{
              headerShown: true,
              headerRight: () => (
                <View style={{ flexDirection: 'row', justifyContent: 'center', alignItems: 'center', gap: 15, marginRight: 10 }}>
                  <TouchableOpacity onPress={() => { /* navigate to notifications */ }}>
                    <Users color={theme.colors.onBackground} size={28} />
                  </TouchableOpacity>
                  <Pressable onPress={() => setIsProfileDrawerOpen(true)}>
                    <Ionicons style={{ }} name="person-circle" size={36} color={theme.colors.onBackground} />
                  </Pressable>
                </View>
              ),
              tabBarIcon: ({ focused }) => (
                <Ionicons
                  name={focused ? 'home' : 'home-outline'}
                  color={focused ? theme.colors.onBackground : inactiveColor}
                  size={28}
                />
              ),
              tabBarLabel: ({ focused }) => (
                <Text style={{ fontSize: 14, fontWeight: '600', color: focused ? theme.colors.onBackground : inactiveColor, textAlign: 'center' }}>
                  Home
                </Text>
              ),
            }}
          />

          {/* ── Practice ── */}
          <Tab.Screen
            name="Collections"
            component={CollectionsScreen}
            options={{
              headerShown: true,
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
              headerSearchBarOptions: {
                placeholder: "Search Collections...",
                onChangeText: (event) => 
                    console.log(event.nativeEvent.text),
              }
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
            name="Explore"
            component={ExploreScreen}
            options={{
              headerShown: false,
              tabBarIcon: ({ focused }) => (
                <Ionicons
                  name={focused ? 'planet' : 'planet-outline'}
                  color={focused ? theme.colors.onBackground : inactiveColor}
                  size={28}
                />
              ),
              tabBarLabel: ({ focused }) => (
                <Text style={{ fontSize: 14, fontWeight: '600', color: focused ? theme.colors.onBackground : inactiveColor, textAlign: 'center' }}>
                  Explore
                </Text>
              ),
            }}
          />
        </Tab.Navigator>
      </Drawer>
  );
}