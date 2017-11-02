<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet 
	version="1.0" 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:ppo="http://tempuri.org/PhotoPropertyOutput.xsd">

<!-- Dec2Hex Template												-->
<!--																-->
<!-- Obtaimed from the comp.text.xml newsgroup						-->
<!--                 												-->
<!--	From: Magnus Henriksson (magnus.henriksson@emw.ericsson.se)	-->
<!--	Subject: Re: XLST Hexadecimal output						-->
<!--	Newsgroups: comp.text.xml									-->
<!--	Date: 2002-12-10 08:29:22 PST								-->
<xsl:template name="Dec2Hex">
	<!-- Recursive template to transform a base-10 number into a base-16 number -->
	<xsl:param name="dec" select="0"/>
	<xsl:variable name="div" select="floor($dec div 16)"/>
	<xsl:variable name="rem" select="$dec - ($div * 16)"/>
	<xsl:choose>
		<xsl:when test="$dec = 0">0</xsl:when>
		<xsl:when test="$dec = 1">1</xsl:when>
		<xsl:when test="$dec = 2">2</xsl:when>
		<xsl:when test="$dec = 3">3</xsl:when>
		<xsl:when test="$dec = 4">4</xsl:when>
		<xsl:when test="$dec = 5">5</xsl:when>
		<xsl:when test="$dec = 6">6</xsl:when>
		<xsl:when test="$dec = 7">7</xsl:when>
		<xsl:when test="$dec = 8">8</xsl:when>
		<xsl:when test="$dec = 9">9</xsl:when>
		<xsl:when test="$dec = 10">A</xsl:when>
		<xsl:when test="$dec = 11">B</xsl:when>
		<xsl:when test="$dec = 12">C</xsl:when>
		<xsl:when test="$dec = 13">D</xsl:when>
		<xsl:when test="$dec = 14">E</xsl:when>
		<xsl:when test="$dec = 15">F</xsl:when>
		<xsl:otherwise>
			<xsl:call-template name="Dec2Hex">
				<xsl:with-param name="dec" select="$div"/>
			</xsl:call-template>
		</xsl:otherwise>
	</xsl:choose>
	<xsl:if test="$div">
		<xsl:call-template name="Dec2Hex">
			<xsl:with-param name="dec" select="$rem"/>
		</xsl:call-template>
	</xsl:if>
</xsl:template>

<xsl:template match="/">
<html>
	<header>
		<title>Digital Image Properties</title>
		<style>
			body { 
				font : 10pt arial,sans serif; 
			}
			.analysis {
				font : bold 12pt arial,sans serif; 
			}
			.analysisFile {
				font : bold italic 12pt arial,sans serif;
			}
			.outputCreation {
				font : bold 10pt arial,sans serif;
				padding-top : 5px;
				padding-bottom : 10px;
			}
			td {
				font : 10pt arial,sans serif;
			}
			td.header {
				font-size : 11pt;
				font-weight : bold;
			}
			td.prettyprint {
				font-style : italic;
			}
		</style>
	</header>
	<body>
		<xsl:apply-templates/> 
	</body>
</html>
</xsl:template>

<xsl:template match="ppo:photoTagProperties">
	<xsl:variable name="imageFileName">
		<xsl:value-of select="ppo:imageFile"/>
	</xsl:variable>
	<div class="analysis">Analysis of 
		<a href="{$imageFileName}"><span class="analysisFile"><xsl:value-of select="ppo:imageFile"/></span></a>
	</div>
	<div class="outputCreation">Created on <xsl:value-of select="ppo:createdLocal"/></div>
	<table cellspacing="0" cellpadding="3" border="1">
		<tr>
			<td class="header">TAG</td>
			<td class="header">CATEGORY</td>
			<td class="header">NAME</td>
			<td class="header">VALUE</td>
		</tr>
		<xsl:apply-templates select="ppo:tagData"/>
	</table>
</xsl:template>

<xsl:template match="ppo:tagData">
	<xsl:apply-templates select="ppo:tagDatum"/>
</xsl:template>

<xsl:template match="ppo:tagDatum">
	<tr>
		<xsl:choose>
			<xsl:when test="(@id) != ''">
				<xsl:apply-templates select="@id"/>
			</xsl:when>
			<xsl:otherwise>
				<td align="center" valign="top"><xsl:text disable-output-escaping="yes">&amp;ndash;</xsl:text></td>
			</xsl:otherwise>
		</xsl:choose>

		<xsl:choose>
			<xsl:when test="(@category) != ''">
				<xsl:apply-templates select="@category"/>
			</xsl:when>
			<xsl:otherwise>
				<td align="center" valign="top"><xsl:text disable-output-escaping="yes">&amp;ndash;</xsl:text></td>
			</xsl:otherwise>
		</xsl:choose>

		<xsl:choose>
			<xsl:when test="(ppo:name) != ''">
				<xsl:apply-templates select="ppo:name"/>
			</xsl:when>
			<xsl:otherwise>
				<td align="center" valign="top"><xsl:text disable-output-escaping="yes">&amp;ndash;</xsl:text></td>
			</xsl:otherwise>
		</xsl:choose>

		<xsl:choose>
			<xsl:when test="(ppo:prettyPrintValue) != ''">
				<xsl:apply-templates select="ppo:prettyPrintValue"/>
			</xsl:when>
			<xsl:when test="(ppo:value) != ''">
				<xsl:apply-templates select="ppo:value"/>
			</xsl:when>
			<xsl:otherwise>
				<td align="center" valign="top"><xsl:text disable-output-escaping="yes">&amp;ndash;</xsl:text></td>
			</xsl:otherwise>
		</xsl:choose>
	</tr>
</xsl:template>

<xsl:template name="pad">
	<xsl:param name="s"/>
	<xsl:param name="len"/>
	<xsl:variable name="spaces">0000</xsl:variable>
	<xsl:value-of select="substring(concat($s, $spaces), 1, $len)"/>
</xsl:template>


<xsl:template match="@id">
	<xsl:variable name="hexval">
		<xsl:call-template name="Dec2Hex"><xsl:with-param name="dec" select="."/></xsl:call-template>
	</xsl:variable>
	<td valign="top" title="{.}">
		<xsl:text>0x</xsl:text>
		<xsl:value-of select="concat(substring('0000', 1, 4 - string-length($hexval)), $hexval)"/>
	</td>
</xsl:template>

<xsl:template match="@category">
	<td valign="top"><xsl:value-of select="."/></td>
</xsl:template>

<xsl:template match="ppo:name">
	<td valign="top" title="{../ppo:description}"><xsl:value-of select="."/></td>
</xsl:template>

<xsl:template match="ppo:prettyPrintValue">
	<td class="prettyprint" valign="top" title="Original Value: {../ppo:value}"><xsl:value-of select="."/></td>
</xsl:template>

<xsl:template match="ppo:value">
	<td valign="top"><xsl:value-of select="."/></td>
</xsl:template>

</xsl:stylesheet>
