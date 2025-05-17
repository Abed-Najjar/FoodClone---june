import 'package:flutter/material.dart';
import 'dart:convert';

class ResponseDebugWidget extends StatelessWidget {
  final dynamic apiResponse;
  final String title;
  
  const ResponseDebugWidget({
    super.key, 
    required this.apiResponse, 
    this.title = 'API Response Debug'
  });

  @override
  Widget build(BuildContext context) {
    String prettyJson = '';
    
    try {
      if (apiResponse is String) {
        final jsonObj = jsonDecode(apiResponse);
        prettyJson = const JsonEncoder.withIndent('  ').convert(jsonObj);
      } else {
        prettyJson = const JsonEncoder.withIndent('  ').convert(apiResponse);
      }
    } catch (e) {
      prettyJson = apiResponse.toString();
    }
    
    return Container(
      padding: const EdgeInsets.all(10),
      margin: const EdgeInsets.all(10),
      decoration: BoxDecoration(
        color: Colors.grey[100],
        borderRadius: BorderRadius.circular(8),
        border: Border.all(color: Colors.grey),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(title, style: const TextStyle(fontWeight: FontWeight.bold)),
          const SizedBox(height: 10),
          SingleChildScrollView(
            scrollDirection: Axis.horizontal,
            child: Container(
              padding: const EdgeInsets.all(10),
              decoration: BoxDecoration(
                color: Colors.black,
                borderRadius: BorderRadius.circular(5),
              ),
              child: SelectableText(
                prettyJson,
                style: const TextStyle(
                  color: Colors.green,
                  fontFamily: 'monospace',
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}
