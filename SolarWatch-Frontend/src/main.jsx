import React from 'react'
import ReactDOM from 'react-dom/client'
import { RouterProvider, createBrowserRouter } from 'react-router-dom'

import Layout from './Pages/Layout/Layout'
import ErrorPage from './Pages/ErrorPage/ErrorPage'
import Home from './Pages/Home/Home'
import Registration, {action as RegistrationAction} from './Components/Authentication/Register'
import Login from './Components/Authentication/Login'
import SolarWatch from './Pages/SolarWatch/SolarWatch'
import './index.css'

const router = createBrowserRouter(
  [
      {
          path: "/",
          element: <Layout/>,
          errorElement: <ErrorPage/>,
          children: [
              {
                  path: "/",
                  element: <Home/>,
              },
              {
                  path: "/solar-watch",
                  element: <SolarWatch/>
              },
              {/*
                  path: "/ads/:id",
                  element: <AdvertisementDetail/>,
                  loader: ({params}) => adLoader(params.id)
              },
              {
                  path: "/carousel",
                  element: <AdvertisementCarousel/>
              }
              ,{
                  path: "/users/:name",
                  element: <Profile/>,
                  loader: ({params}) => profileLoader(params.name)
              */} ,
              {
                path: "/login",
                element: <Login />
              },
              {
                  path: "/registration",
                  element: <Registration />,
                  action: RegistrationAction
              }
          ],
      },
  ]);

ReactDOM.createRoot(document.getElementById("root")).render(
  <React.StrictMode>
      <RouterProvider router={router}/>
  </React.StrictMode>
)
