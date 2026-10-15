import 'package:flutter/material.dart';
import '../../core/utils/formatters.dart';

/// A performance-first VND text widget that guarantees zero RenderFlex overflows
/// by wrapping numbers in FittedBox with tabular figures alignment.
class VndText extends StatelessWidget {
  final num? amount;
  final TextStyle? style;
  final bool compact;
  final Alignment alignment;
  final String? prefix;
  final String? suffix;

  const VndText(
    this.amount, {
    super.key,
    this.style,
    this.compact = false,
    this.alignment = Alignment.centerLeft,
    this.prefix,
    this.suffix,
  });

  @override
  Widget build(BuildContext context) {
    final textTheme = Theme.of(context).textTheme;
    final effectiveStyle = (style ?? textTheme.titleMedium)?.copyWith(
      fontFeatures: const [FontFeature.tabularFigures()],
    );

    final formatted = Formatters.formatVndSafe(amount, compact: compact);
    final displayText = '${prefix ?? ''}$formatted${suffix ?? ''}';

    return FittedBox(
      fit: BoxFit.scaleDown,
      alignment: alignment,
      child: Text(
        displayText,
        style: effectiveStyle,
        maxLines: 1,
      ),
    );
  }
}
