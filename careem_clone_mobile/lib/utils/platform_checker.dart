import 'package:flutter/foundation.dart';

class PlatformChecker {
  static bool get isWeb => kIsWeb;
  
  static bool get isIOS => !kIsWeb && defaultTargetPlatform == TargetPlatform.iOS;
  
  static bool get isAndroid => !kIsWeb && defaultTargetPlatform == TargetPlatform.android;
  
  static bool get isDesktop => !kIsWeb && (isMacOS || isWindows || isLinux);
  
  static bool get isMacOS => !kIsWeb && defaultTargetPlatform == TargetPlatform.macOS;
  
  static bool get isWindows => !kIsWeb && defaultTargetPlatform == TargetPlatform.windows;
  
  static bool get isLinux => !kIsWeb && defaultTargetPlatform == TargetPlatform.linux;
  
  static String get currentPlatform {
    if (isWeb) return 'Web';
    if (isAndroid) return 'Android';
    if (isIOS) return 'iOS';
    if (isMacOS) return 'macOS';
    if (isWindows) return 'Windows';
    if (isLinux) return 'Linux';
    return 'Unknown';
  }
}
