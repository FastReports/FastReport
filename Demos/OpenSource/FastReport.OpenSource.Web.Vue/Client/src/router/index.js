import Vue from 'vue'
import Router from 'vue-router'
import Reports from '@/components/Reports'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'Reports',
      component: Reports
    }
  ]
})
