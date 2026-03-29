import { createMaterialTopTabNavigator } from '@react-navigation/material-top-tabs';
import { Pressable, Text, TouchableOpacity, View } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import Ionicons from '@expo/vector-icons/Ionicons';
import useAppTheme from '../../theme';
import { HomeScreen } from './home.screen';
import { PublishedScreen } from './published.screen';
import { useRef, useState } from 'react';
import { CollectionsScreen } from './collections.screen';

const Tab = createMaterialTopTabNavigator();

export function CollectionTopTabs() {
  const theme = useAppTheme();
  const insets = useSafeAreaInsets();
  const navRef = useRef<any>(null);
  const [activeIndex, setActiveIndex] = useState(0);

  const tabs = [
    { name: 'HomeMain', label: 'Collections' },
    { name: 'Profile',  label: 'Published', badge: 3 },
  ];

  return (
    <Tab.Navigator
    
          screenOptions={{
            tabBarActiveTintColor: theme.colors.onBackground,
            tabBarInactiveTintColor: theme.colors.onBackground,
            tabBarItemStyle: {
              
            },
            tabBarLabelStyle: {
              
            },
            tabBarStyle: {
              backgroundColor: theme.colors.background2,
              height: 50 + insets.bottom + 10,
              paddingTop: insets.bottom + 10,
              // paddingBottom: Math.max(insets.bottom, 10) + 10,
              // paddingTop: 10,
              // paddingLeft: 5,
              // paddingRight: 5,
              // borderTopColor: theme.colors.elevation2,
              // borderTopWidth: 0.2,
            },
          }}>
      <Tab.Screen name="Collections" component={CollectionsScreen} />
      <Tab.Screen name="Published" component={PublishedScreen} />
    </Tab.Navigator>
  );
}