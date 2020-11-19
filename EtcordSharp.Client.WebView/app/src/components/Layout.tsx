import React from 'react'
import { Layout as AntLayout } from 'antd'
import styled from 'styled-components'

const Layout = styled(AntLayout)`
    height: 100%;
    width: 100%;
`

const BasicLayout = ({
    children,
}: {
    children: React.ReactNode | React.ReactNode[]
}) => <Layout>{children}</Layout>

export default BasicLayout
