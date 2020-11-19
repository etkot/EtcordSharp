import React from 'react'
import { Layout as AntLayout } from 'antd'
import styled from 'styled-components'

import LayoutWrapper from './components/Layout'
import Menu from './components/Menu'
import Sidebar from './components/Sidebar'

const Content = styled(AntLayout.Content)``
const Header = styled(AntLayout.Header)`
    padding: 0;
    display: flex;
`

const App = () => (
    <LayoutWrapper>
        <Header>
            <Menu />
        </Header>
        <AntLayout>
            <Sidebar />
            <Content>Jei</Content>
        </AntLayout>
    </LayoutWrapper>
)

export default App
