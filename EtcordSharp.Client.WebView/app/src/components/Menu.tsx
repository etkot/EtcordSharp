import React from 'react'
import { Menu as AntMenu } from 'antd'
import styled from 'styled-components'

import Logo from '../assets/logo.png'

const MenuWrapper = styled(AntMenu)`
    z-index: 10;
    position: relative;
    flex: 1;
    background-color: #131313;
`
const Item = styled(AntMenu.Item)``

const LogoContainer = styled.div`
    height: 64px;
    padding: 0 16px;
    padding-left: 16px;
    border-bottom: 1px solid #303030;
    background-color: #131313;

    display: flex;
    align-items: center;
    justify-content: center;
`

const Img = styled.img`
    height: 44px;
    width: auto;
    filter: invert();
`

const Menu = () => (
    <>
        <LogoContainer>
            <Img src={Logo} />
        </LogoContainer>
        <MenuWrapper mode="horizontal" defaultSelectedKeys={['1']}>
            <Item key="1">Stuff here?</Item>
        </MenuWrapper>
    </>
)

export default Menu
