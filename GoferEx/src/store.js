import _ from 'lodash'
import uuid from 'uuid'
import Vue from 'vue'
import Vuex from 'vuex'
import defaultUsers from './default-users.js'
import axios from 'axios'

Vue.use(Vuex)

const myApi = axios.create({
  baseURL: 'http://localhost:1995/',
  withCredentials: true,
  headers: {
    'Accept': 'application/json',
    'Content-Type': 'application/json'
  }
})

const localStorage = window.localStorage

function updateStorage (users) {
  localStorage.setItem('users', JSON.stringify(users))
}

myApi.get('api/contact').then(res => {
  if (res.data.length > 0) {
    localStorage.setItem('users', res.data)
  }
}).catch(e => {
  alert('Got error while fetching users from server ' + e.message)
})

var existUsers = []

var initUsers = function () {
  if (localStorage.getItem('users') !== null) {
    existUsers = JSON.parse(localStorage.getItem('users'))
  } else {
    existUsers = defaultUsers
  }
}
initUsers()

export default new Vuex.Store({
  state: {
    users: existUsers,
    toasts: [],
    currentUser: null,
    syncedTokens: [],
    authSchemes: []
  },
  mutations: {
    addUser (state, user) {
      if (!user.id) {
        user.id = uuid()
      }
      myApi.post('api/contact', user).then(res => {
        state.users = res.data
      }).catch(e => {
        toast = { id: user.id, message: 'Error while adding new contact ' + e.Message, isPositive: false }
        state.toasts.push(toast)
        setTimeout(() => {
          state.toasts = _.remove(state.toasts, x => x.id !== toast.id)
        }, 3000)
      })
      let users = state.users

      let idx = _.findIndex(users, { id: user.id })
      let toast
      if (idx !== -1) {
        users.splice(idx, 1, user)
        toast = { id: user.id, message: 'User updated', isPositive: true }
      } else {
        users.push(user)
        toast = { id: user.id, message: 'User added', isPositive: true }
      }

      state.users = users
      state.toasts.push(toast)
      setTimeout(() => {
        state.toasts = _.remove(state.toasts, x => x.id !== toast.id)
      }, 3000)

      updateStorage(state.users)
    },
    removeUser (state, user) {
      console.log('Delete storage func')
      let reqUrl = 'api/contact/' + user.id
      myApi.delete(reqUrl).then(res => {
        state.users = res.data
      }).catch(e => {
        let toast = { id: user.id, message: 'Error while deleting user ' + e.message, isPositive: false }
        state.toasts.push(toast)
        setTimeout(() => {
          state.toasts = _.remove(state.toasts, x => x.id !== toast.id)
        }, 3000)
      })
      state.users = _.remove(state.users, (u) => u.id !== user.id)
      state.currentUser = null

      let toast = { id: uuid(), message: 'User removed', isPositive: true }
      state.toasts.push(toast)
      setTimeout(() => {
        state.toasts = _.remove(state.toasts, x => x.id !== toast.id)
      }, 3000)

      updateStorage(state.users)
    },
    selectUser (state, user) {
      console.log('Select storage func')
      let reqUrl = 'api/contact/' + user.id
      myApi.get(reqUrl).then(res => {
        state.currentUser = res.data
      }).catch(e => {
        let toast = { id: uuid(), message: 'Error while getting server info for user ' + e.meesage, isPositive: false }
        state.toasts.push(toast)
        setTimeout(() => {
          state.toasts = _.remove(state.toasts, x => x.id !== toast.id)
        }, 3000)
      })
    },
    removeToast (state, toast) {
      state.toasts = _.remove(state.toasts, x => x.id !== toast.id)
    },
    loginChallenge (state) {
      myApi.get('login')
        .then(res => {
          res.data.forEach(function (scheme) {
            scheme.LoginChallenge = 'http://localhost:1995/login' + scheme.LoginAuthUri
            state.authSchemes.push(scheme)
          })
        }).catch(e => {
          state.toasts.push('toast')
        })
    },
    syncContacts (state) {
      myApi.get('api/sync')
        .then(res => {
          console.log(res.data)
        }).catch(e => {
          state.toasts.push('toast')
        })
    }
  }
})
