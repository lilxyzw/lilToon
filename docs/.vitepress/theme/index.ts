import type { EnhanceAppContext } from 'vitepress'
import DefaultTheme from 'vitepress/theme'
import './styles/twemoji.css'
import 'lightbox2/dist/css/lightbox.min.css'
import './styles/custom.css'

export default {
  ...DefaultTheme,
  enhanceApp(ctx: EnhanceAppContext) {
    if (!import.meta.env.SSR) {
      var script = document.createElement('script');
      script.src = '/lilToon/scripts/init.js';
      document.body.appendChild(script);
    }
  }
}