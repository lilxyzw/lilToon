import type { PluginSimple } from "markdown-it";
import { twemoji } from './twemoji';

const markdownItTwemoji: PluginSimple = (md) => {
  var defaultRender =
    md.renderer.rules.text ||
    function (tokens, idx, options, env, self) {
      return self.renderToken(tokens, idx, options);
    };

  md.renderer.rules.text = function (tokens, idx, options, env, self) {
    tokens[idx].content = twemoji.parse(tokens[idx].content);
    return defaultRender(tokens, idx, options, env, self);
  }
};

export default markdownItTwemoji;