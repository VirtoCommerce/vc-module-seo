import * as blades from "./pages";
import * as locales from "./locales";
import { defineAppModule, registerExternalWidget, useApiClient } from "@vc-shell/framework";
import { defineComponent, h, markRaw } from "vue";
import StoreSeoWidget from "./widgets/StoreSeoWidget.vue";
import { BrokenLinksClient, RedirectRulesClient } from "../api_client/virtocommerce.seo";
import "./styles.css";

// The AngularJS module's widgets on the store blade ("storeDetail"): redirect rules, broken links, SEO explain.
// (The store's own SEO records widget belongs to the Store module, which registered it in AngularJS too.)
const widgets = [
  {
    id: "SeoStoreRedirectRules",
    title: "Redirect rules",
    props: {
      icon: "lucide-milestone",
      title: "SEO.WIDGETS.REDIRECT_RULES",
      blade: "SeoRedirectRules",
      count: async (storeId: string) =>
        (await (await useApiClient(RedirectRulesClient).getApiClient()).search({ storeId, isActive: true, take: 0 })).totalCount,
    },
  },
  {
    id: "SeoStoreBrokenLinks",
    title: "Broken links",
    props: {
      icon: "lucide-unlink",
      title: "SEO.WIDGETS.BROKEN_LINKS",
      blade: "SeoBrokenLinks",
      count: async (storeId: string) =>
        (await (await useApiClient(BrokenLinksClient).getApiClient()).search({ storeId, status: "Active", take: 0 })).totalCount,
    },
  },
  { id: "SeoStoreExplain", title: "SEO explain", props: { icon: "lucide-search-check", title: "SEO.WIDGETS.SEO_EXPLAIN", blade: "SeoExplain" } },
];
for (const widget of widgets) {
  registerExternalWidget({
    id: widget.id,
    title: widget.title,
    component: markRaw(defineComponent({ name: widget.id, setup: () => () => h(StoreSeoWidget, widget.props) })),
    targetBlades: ["StoreDetails"],
    // not on a store that isn't saved yet
    isVisible: (blade) => !!blade?.param,
  });
}

export default defineAppModule({
  blades,
  locales,
});
