import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'auth_wrapper.dart';

class TutorialScreen extends StatefulWidget {
  const TutorialScreen({super.key});

  @override
  State<TutorialScreen> createState() => _TutorialScreenState();
}

class _TutorialScreenState extends State<TutorialScreen> {
  final PageController _pageController = PageController();
  int _currentPage = 0;
  final int _numPages = 3;
  
  final List<TutorialPage> _pages = [
    const TutorialPage(
      title: 'Welcome to Careem Clone',
      description: 'Your one-stop solution for food delivery',
      image: Icons.restaurant,
      color: Colors.green,
    ),
    const TutorialPage(
      title: 'Find Best Restaurants',
      description: 'Browse through a variety of restaurants and cuisines',
      image: Icons.search,
      color: Colors.orange,
    ),
    const TutorialPage(
      title: 'Fast Delivery',
      description: 'Get your favorite food delivered quickly to your doorstep',
      image: Icons.delivery_dining,
      color: Colors.blue,
    ),
  ];
  
  void _onPageChanged(int page) {
    setState(() {
      _currentPage = page;
    });
  }

  void _markTutorialAsShown() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setBool('tutorialShown', true);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Stack(
        children: [
          PageView.builder(
            physics: const ClampingScrollPhysics(),
            controller: _pageController,
            onPageChanged: _onPageChanged,
            itemCount: _numPages,
            itemBuilder: (context, index) {
              return _pages[index];
            },
          ),
          Positioned(
            bottom: 80,
            left: 0,
            right: 0,
            child: Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: List.generate(
                _numPages,
                (index) => AnimatedContainer(
                  duration: const Duration(milliseconds: 300),
                  height: 10,
                  width: _currentPage == index ? 30 : 10,
                  margin: const EdgeInsets.symmetric(horizontal: 5),
                  decoration: BoxDecoration(
                    color: _currentPage == index
                        ? Theme.of(context).colorScheme.primary
                        : Colors.grey.shade300,
                    borderRadius: BorderRadius.circular(5),
                  ),
                ),
              ),
            ),
          ),
          Positioned(
            bottom: 20,
            left: 0,
            right: 0,
            child: Padding(
              padding: const EdgeInsets.symmetric(horizontal: 20),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  _currentPage > 0
                      ? TextButton(
                          onPressed: () {
                            _pageController.previousPage(
                              duration: const Duration(milliseconds: 300),
                              curve: Curves.easeInOut,
                            );
                          },
                          child: const Text('Previous'),
                        )
                      : const SizedBox.shrink(),
                  ElevatedButton(
                    onPressed: () {
                      if (_currentPage == _numPages - 1) {
                        _markTutorialAsShown();
                        Navigator.of(context).pushReplacement(
                          MaterialPageRoute(
                            builder: (_) => const AuthWrapper(),
                          ),
                        );
                      } else {
                        _pageController.nextPage(
                          duration: const Duration(milliseconds: 300),
                          curve: Curves.easeInOut,
                        );
                      }
                    },
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Theme.of(context).colorScheme.primary,
                      foregroundColor: Colors.white,
                      padding: const EdgeInsets.symmetric(
                        horizontal: 20,
                        vertical: 10,
                      ),
                    ),
                    child: Text(
                      _currentPage == _numPages - 1 ? 'Get Started' : 'Next',
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  @override
  void dispose() {
    _pageController.dispose();
    super.dispose();
  }
}

class TutorialPage extends StatelessWidget {
  final String title;
  final String description;
  final IconData image;
  final Color color;

  const TutorialPage({
    super.key,
    required this.title,
    required this.description,
    required this.image,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(40.0),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(
            image,
            size: 120,
            color: color,
          ),
          const SizedBox(height: 40),
          Text(
            title,
            style: const TextStyle(
              fontSize: 28,
              fontWeight: FontWeight.bold,
            ),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 20),
          Text(
            description,
            style: const TextStyle(
              fontSize: 18,
              color: Colors.grey,
            ),
            textAlign: TextAlign.center,
          ),
        ],
      ),
    );
  }
}
