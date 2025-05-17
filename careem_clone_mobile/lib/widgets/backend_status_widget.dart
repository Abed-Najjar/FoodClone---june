import 'package:flutter/material.dart';
import '../utils/constants.dart';

class BackendStatusWidget extends StatefulWidget {
  final VoidCallback? onRetry;
  
  const BackendStatusWidget({super.key, this.onRetry});

  @override
  State<BackendStatusWidget> createState() => _BackendStatusWidgetState();
}

class _BackendStatusWidgetState extends State<BackendStatusWidget> {
  bool _isLoading = false;
  String _statusMessage = "Checking backend status...";
  bool _isConnected = false;

  @override
  void initState() {
    super.initState();
    _checkBackendStatus();
  }

  Future<void> _checkBackendStatus() async {
    setState(() {
      _isLoading = true;
      _statusMessage = "Checking backend status...";
    });

    try {
      final workingUrl = await AppConstants.findWorkingUrl();
      
      setState(() {
        _isLoading = false;
        _isConnected = workingUrl != null;
        _statusMessage = AppConstants.getBackendStatusMessage();
      });
    } catch (e) {
      setState(() {
        _isLoading = false;
        _isConnected = false;
        _statusMessage = "Error checking backend status: $e";
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(10),
      margin: const EdgeInsets.all(10),
      decoration: BoxDecoration(
        color: _isConnected ? Colors.green[100] : Colors.red[100],
        borderRadius: BorderRadius.circular(8),
        border: Border.all(
          color: _isConnected ? Colors.green : Colors.red,
          width: 1,
        ),
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Row(
            children: [
              Icon(
                _isConnected ? Icons.check_circle : Icons.error,
                color: _isConnected ? Colors.green : Colors.red,
              ),
              const SizedBox(width: 10),
              Expanded(
                child: Text(
                  _statusMessage,
                  style: TextStyle(
                    color: _isConnected ? Colors.green[900] : Colors.red[900],
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 10),
          if (_isLoading)
            const LinearProgressIndicator()
          else
            ElevatedButton(
              onPressed: () {
                _checkBackendStatus();
                if (widget.onRetry != null) widget.onRetry!();
              },
              child: const Text('Retry Connection'),
            ),
        ],
      ),
    );
  }
}
