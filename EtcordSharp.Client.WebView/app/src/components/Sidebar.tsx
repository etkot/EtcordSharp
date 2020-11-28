import React from 'react'
import { Layout as AntLayout, Menu } from 'antd'
import styled from 'styled-components'
import {
    AppstoreOutlined,
    MailOutlined,
    SettingOutlined,
} from '@ant-design/icons'

const Sider = styled(AntLayout.Sider)`
    height: 100%;
    background-color: #1f1f1f;
`

const Sidebar = () => (
    <Sider breakpoint="lg" collapsedWidth="0">
        <Menu
            style={{ height: '100%' }}
            defaultSelectedKeys={['1']}
            defaultOpenKeys={['sub1']}
            mode="inline"
        >
            <Menu.SubMenu
                key="sub1"
                title={
                    <span>
                        <MailOutlined />
                        <span>Navigation One</span>
                    </span>
                }
            >
                <Menu.ItemGroup key="g1" title="Item 1">
                    <Menu.Item key="1">Option 1</Menu.Item>
                    <Menu.Item key="2">Option 2</Menu.Item>
                </Menu.ItemGroup>
                <Menu.ItemGroup key="g2" title="Item 2">
                    <Menu.Item key="3">Option 3</Menu.Item>
                    <Menu.Item key="4">Option 4</Menu.Item>
                </Menu.ItemGroup>
            </Menu.SubMenu>
            <Menu.SubMenu
                key="sub2"
                icon={<AppstoreOutlined />}
                title="Navigation Two"
            >
                <Menu.Item key="5">Option 5</Menu.Item>
                <Menu.Item key="6">Option 6</Menu.Item>
                <Menu.SubMenu key="sub3" title="Menu.Submenu">
                    <Menu.Item key="7">Option 7</Menu.Item>
                    <Menu.Item key="8">Option 8</Menu.Item>
                </Menu.SubMenu>
            </Menu.SubMenu>
            <Menu.SubMenu
                key="sub4"
                title={
                    <span>
                        <SettingOutlined />
                        <span>Navigation Three</span>
                    </span>
                }
            >
                <Menu.Item key="9">Option 9</Menu.Item>
                <Menu.Item key="10">Option 10</Menu.Item>
                <Menu.Item key="11">Option 11</Menu.Item>
                <Menu.Item key="12">Option 12</Menu.Item>
            </Menu.SubMenu>
        </Menu>
    </Sider>
)

export default Sidebar
